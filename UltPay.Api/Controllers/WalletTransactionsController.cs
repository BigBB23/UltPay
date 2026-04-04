using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Infrastructure.Persistence;

namespace UltPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletTransactionsController : ControllerBase
{
    private readonly UltPayDbContext _context;

    public WalletTransactionsController(UltPayDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var transactions = await _context.WalletTransactions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("wallet/{walletId:guid}")]
    public async Task<IActionResult> GetByWallet(Guid walletId)
    {
        var transactions = await _context.WalletTransactions
            .Where(x => x.WalletId == walletId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(transactions);
    }
}