
namespace UltPay.Domain.Entities;

public class Quote
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SourceCountry { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = string.Empty;
    public string SourceCurrency { get; set; } = string.Empty;
    public string DestinationCurrency { get; set; } = string.Empty;
    public decimal SourceAmount { get; set; }
    public decimal DestinationAmount { get; set; }
    public decimal FxRate { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal MarginApplied { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
