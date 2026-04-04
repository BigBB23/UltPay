namespace UltPay.Contracts.Requests;

public class CreateTransferRequest
{
    public Guid UserId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public Guid QuoteId { get; set; }

    public decimal SourceAmount { get; set; }
    public string SourceCurrency { get; set; } = string.Empty;
    public string DestinationCurrency { get; set; } = string.Empty;
}