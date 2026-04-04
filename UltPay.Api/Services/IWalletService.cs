using UltPay.Domain.Entities;

namespace UltPay.Api.Services;

public interface IWalletService
{
    Task<Wallet> GetOrCreateWalletAsync(Guid userId, string currencyCode);
    Task<bool> CreditAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description);
    Task<bool> ReserveAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description);
    Task<bool> CommitReservedAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description);
    Task<bool> ReleaseReservedAsync(Guid userId, string currencyCode, decimal amount, string referenceType, Guid? referenceId, string description);
}