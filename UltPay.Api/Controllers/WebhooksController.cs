using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Api.Providers.Flutterwave;
using UltPay.Infrastructure.Persistence;
using UltPay.Domain.Entities;

namespace UltPay.Api.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class WebhooksController : ControllerBase
    {
        private readonly UltPayDbContext _context;
        private readonly ILogger<WebhooksController> _logger;
        private readonly IConfiguration _configuration;

        public WebhooksController(
            UltPayDbContext context,
            ILogger<WebhooksController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("flutterwave")]
        public async Task<IActionResult> Flutterwave([FromBody] FlutterwaveWebhookRequest request)
        {
            var secretHash = Request.Headers["verif-hash"].FirstOrDefault();
            var expectedHash = _configuration["Flutterwave:WebhookSecret"];

            if (!string.IsNullOrWhiteSpace(expectedHash) && secretHash != expectedHash)
            {
                _logger.LogWarning("Invalid Flutterwave webhook signature.");
                return Unauthorized();
            }

            _logger.LogInformation("Flutterwave webhook received: {@Request}", request);

            var providerReference =
                request.data?.reference ??
                request.data?.tx_ref ??
                request.tx_ref;

            if (string.IsNullOrWhiteSpace(providerReference))
            {
                return BadRequest(new { message = "Missing provider reference" });
            }

            var transfer = await _context.Transfers
                .FirstOrDefaultAsync(x =>
                    x.ProviderReference == providerReference ||
                    x.CorrelationId == providerReference);

            if (transfer == null)
            {
                _logger.LogWarning("Transfer not found for webhook reference: {Reference}", providerReference);
                return NotFound(new { message = "Transfer not found" });
            }

            if (transfer.Status == "SUCCESS" || transfer.Status == "FAILED")
            {
                _logger.LogInformation("Transfer already finalized: {TransferId}", transfer.Id);
                return Ok(new { message = "Already finalized" });
            }

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(x =>
                    x.UserId == transfer.UserId &&
                    x.CurrencyCode == transfer.SourceCurrency);

            if (wallet == null)
            {
                return BadRequest(new { message = "Wallet not found" });
            }

            var webhookStatus = (request.data?.status ?? request.status ?? string.Empty).ToUpperInvariant();

            if (webhookStatus == "SUCCESSFUL" || webhookStatus == "SUCCESS")
            {
                var balanceBefore = wallet.AvailableBalance;

                wallet.ReservedBalance -= transfer.SourceAmount;
                wallet.UpdatedAtUtc = DateTime.UtcNow;

                _context.WalletTransactions.Add(new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    UserId = wallet.UserId,
                    CurrencyCode = wallet.CurrencyCode,
                    Amount = transfer.SourceAmount,
                    Type = "DEBIT",
                    ReferenceType = "TRANSFER",
                    ReferenceId = transfer.Id,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.AvailableBalance,
                    Narration = $"Transfer finalized by webhook: {transfer.Id}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                transfer.Status = "SUCCESS";
                transfer.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Transfer finalized as SUCCESS" });
            }

            if (webhookStatus == "FAILED" || webhookStatus == "ERROR")
            {
                var balanceBefore = wallet.AvailableBalance;

                wallet.ReservedBalance -= transfer.SourceAmount;
                wallet.AvailableBalance += transfer.SourceAmount;
                wallet.UpdatedAtUtc = DateTime.UtcNow;

                _context.WalletTransactions.Add(new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    UserId = wallet.UserId,
                    CurrencyCode = wallet.CurrencyCode,
                    Amount = transfer.SourceAmount,
                    Type = "RELEASE",
                    ReferenceType = "TRANSFER",
                    ReferenceId = transfer.Id,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.AvailableBalance,
                    Narration = $"Transfer failed by webhook: {transfer.Id}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                transfer.Status = "FAILED";
                transfer.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Transfer finalized as FAILED" });
            }

            _logger.LogInformation("Webhook received non-final status {Status} for transfer {TransferId}", webhookStatus, transfer.Id);

            return Ok(new { message = "Webhook received, no final action taken" });
        }
    }
}
