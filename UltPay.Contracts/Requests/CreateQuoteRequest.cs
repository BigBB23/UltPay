namespace UltPay.Contracts.Requests;

public class CreateQuoteRequest
{
    public Guid UserId { get; set; }
    public decimal SourceAmount { get; set; }
    public string SourceCurrency { get; set; } = string.Empty;
    public string DestinationCurrency { get; set; } = string.Empty;
}