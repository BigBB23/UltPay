using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;
using UltPay.Contracts.Requests;

namespace UltPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly UltPayDbContext _context;

    public WalletsController(UltPayDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var wallets = await _context.Wallets
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CurrencyCode)
            .ToListAsync();

        return Ok(wallets);
    }
    [HttpPost("fund")]
public async Task<IActionResult> Fund(FundWalletRequest request)
{
    if (request.Amount <= 0)
        return BadRequest(new { message = "Amount must be greater than zero." });

    var wallet = await _context.Wallets
        .FirstOrDefaultAsync(x =>
            x.UserId == request.UserId &&
            x.CurrencyCode == request.CurrencyCode);

    decimal balanceBefore = 0m;

    if (wallet == null)
    {
        wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            CurrencyCode = request.CurrencyCode,
            AvailableBalance = request.Amount,
            ReservedBalance = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Wallets.Add(wallet);
    }
    else
    {
        balanceBefore = wallet.AvailableBalance;
        wallet.AvailableBalance += request.Amount;
        wallet.UpdatedAtUtc = DateTime.UtcNow;
    }

    var balanceAfter = wallet.AvailableBalance;

    var walletTransaction = new WalletTransaction
    {
        Id = Guid.NewGuid(),
        WalletId = wallet.Id,
        UserId = request.UserId,
        CurrencyCode = request.CurrencyCode,
        Amount = request.Amount,
        Type = "CREDIT",
        ReferenceType = "FUNDING",
        ReferenceId = null,
        BalanceBefore = balanceBefore,
        BalanceAfter = balanceAfter,
        Narration = "Wallet funded",
        CreatedAtUtc = DateTime.UtcNow
    };

        _context.WalletTransactions.Add(walletTransaction);

        await _context.SaveChangesAsync();

        return Ok(wallet);
    }
}