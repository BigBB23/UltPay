
namespace UltPay.Domain.Entities;

public class Transfer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public Guid QuoteId { get; set; }
    public decimal SourceAmount { get; set; }
    public decimal DestinationAmount { get; set; }
    public string SourceCurrency { get; set; } = string.Empty;
    public string DestinationCurrency { get; set; } = string.Empty;
    public decimal FeeAmount { get; set; }
    public decimal FxRate { get; set; }
    public string Status { get; set; } = "CREATED";
    public string Provider { get; set; } = "Flutterwave";
    public string ProviderReference { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
