using Microsoft.EntityFrameworkCore;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;

namespace UltPay.Api.Services;

public class WalletService : IWalletService
{
    private readonly UltPayDbContext _context;

    public WalletService(UltPayDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> GetOrCreateWalletAsync(Guid userId, string currencyCode)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CurrencyCode == currencyCode);

        if (wallet != null)
            return wallet;

        wallet = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CurrencyCode = currencyCode,
            AvailableBalance = 0,
            ReservedBalance = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();
        return wallet;
    }
    public async Task<bool> CreditAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description)
    {
        if (amount <= 0) return false;

        var wallet = await GetOrCreateWalletAsync(userId, currencyCode);
        var before = wallet.AvailableBalance;

        wallet.AvailableBalance += amount;
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        _context.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            UserId = userId,
            CurrencyCode = currencyCode,
            EntryType = "CREDIT",
            Amount = amount,
            BalanceBefore = before,
            BalanceAfter = wallet.AvailableBalance,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Description = description,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReserveAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description)
    {
        if (amount <= 0) return false;

        var wallet = await GetOrCreateWalletAsync(userId, currencyCode);

        if (wallet.AvailableBalance < amount)
            return false;

        var before = wallet.AvailableBalance;

        wallet.AvailableBalance -= amount;
        wallet.ReservedBalance += amount;
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        _context.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            UserId = userId,
            CurrencyCode = currencyCode,
            EntryType = "RESERVE",
            Amount = amount,
            BalanceBefore = before,
            BalanceAfter = wallet.AvailableBalance,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Description = description,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CommitReservedAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description)
    {
        if (amount <= 0) return false;

        var wallet = await GetOrCreateWalletAsync(userId, currencyCode);

        if (wallet.ReservedBalance < amount)
            return false;

        wallet.ReservedBalance -= amount;
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        _context.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            UserId = userId,
            CurrencyCode = currencyCode,

            EntryType = "DEBIT",
            Amount = amount,
            BalanceBefore = wallet.AvailableBalance,
            BalanceAfter = wallet.AvailableBalance,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Description = description,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReleaseReservedAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description)
    {
        if (amount <= 0) return false;

        var wallet = await GetOrCreateWalletAsync(userId, currencyCode);

        if (wallet.ReservedBalance < amount)
            return false;

        var before = wallet.AvailableBalance;

        wallet.ReservedBalance -= amount;
        wallet.AvailableBalance += amount;
        wallet.UpdatedAtUtc = DateTime.UtcNow;

        _context.LedgerEntries.Add(new LedgerEntry
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            UserId = userId,
            CurrencyCode = currencyCode,
            EntryType = "RELEASE",
            Amount = amount,
            BalanceBefore = before,
            BalanceAfter = wallet.AvailableBalance,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            Description = description,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }
}