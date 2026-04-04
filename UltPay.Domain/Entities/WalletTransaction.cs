namespace UltPay.Domain.Entities;

public class WalletTransaction
{
    public Guid Id { get; set; }

    public Guid WalletId { get; set; }

    public Guid UserId { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty;
    // CREDIT, DEBIT, RESERVE, RELEASE

    public string ReferenceType { get; set; } = string.Empty;
    // FUNDING, TRANSFER, REFUND

    public Guid? ReferenceId { get; set; }

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public string Narration { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}