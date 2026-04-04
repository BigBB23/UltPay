
namespace UltPay.Domain.Entities;

public class Beneficiary
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string BankCode { get; set; } 
    public string AccountNumber { get; set; }
    public string ? MobileMoneyNumber { get; set; } 
    public string ? MobileMoneyProvider{ get; set; }
    public string PayoutMethod { get; set; } 
    public DateTime CreatedAtUtc { get; set; } 
}
