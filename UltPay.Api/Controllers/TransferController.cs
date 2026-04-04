using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Api.Services;
using UltPay.Contracts.Requests;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;

namespace UltPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly UltPayDbContext _context;
    private readonly ITransferExecutionService _transferExecutionService;

    public TransfersController(
        UltPayDbContext context,
        ITransferExecutionService transferExecutionService)
    {
        _context = context;
        _transferExecutionService = transferExecutionService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var transfers = await _context.Transfers
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(transfers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transfer = await _context.Transfers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (transfer == null)
            return NotFound(new { message = "Transfer not found." });

        return Ok(transfer);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetTransfersByUser(Guid userId)
    {
        var transfers = await _context.Transfers
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(transfers);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTransferRequest request)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(x => x.Id == request.QuoteId);

        if (quote == null)
            return NotFound(new { message = "Quote not found." });

        if (!string.Equals(quote.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Quote is not active." });

        if (quote.UserId != request.UserId)
            return BadRequest(new { message = "Quote does not belong to this user." });

        var beneficiary = await _context.Beneficiaries
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (beneficiary == null)
            return BadRequest(new { message = "Beneficiary does not exist for this user." });

        var transfer = new Transfer
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            BeneficiaryId = beneficiary.Id,
            QuoteId = quote.Id,
            SourceAmount = quote.SourceAmount,
            DestinationAmount = quote.DestinationAmount,
            SourceCurrency = quote.SourceCurrency,
            DestinationCurrency = quote.DestinationCurrency,
            FeeAmount = quote.FeeAmount,
            FxRate = quote.FxRate,
            Status = "CREATED",
            Provider = "Flutterwave",
            ProviderReference = string.Empty,
            CorrelationId = Guid.NewGuid().ToString(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, transfer);
    }



    
    [HttpPost("{id:guid}/execute")]
    public async Task<IActionResult> Execute(Guid id)
    {
        var result = await _transferExecutionService.ExecuteTransferAsync(id);

        return result switch
        {
            "SUCCESS" => Ok(new { message = "Transfer executed successfully." }),
            "PROCESSING" => Ok(new { message = "Transfer submitted and is processing." }),
            "NOT_FOUND" => NotFound(new { message = "Transfer not found" }),
            "ALREADY_PROCESSED" => BadRequest(new { message = "Already processed" }),
            "INSUFFICIENT_FUNDS" => BadRequest(new { message = "Insufficient funds" }),
            "WALLET_NOT_FOUND" => BadRequest(new { message = "Wallet not found" }),
            "BENEFICIARY_NOT_FOUND" => BadRequest(new { message = "Beneficiary not found" }),
            _ => BadRequest(new { message = result })
        };

    }
}
