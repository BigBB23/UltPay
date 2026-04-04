namespace UltPay.Contracts.Requests;

public class UpdateBeneficiaryRequest
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;

    public string PayoutMethod { get; set; } = string.Empty;

    public string? BankCode { get; set; }
    public string? AccountNumber { get; set; }

    public string? MobileMoneyProvider { get; set; }
    public string? MobileMoneyNumber { get; set; }
}