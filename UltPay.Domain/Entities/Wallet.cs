namespace UltPay.Domain.Entities;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal AvailableBalance { get; set; }
    public decimal ReservedBalance { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}