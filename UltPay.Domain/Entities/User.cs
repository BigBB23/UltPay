
namespace UltPay.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CountryOfResidence { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "en";
    public string PasswordHash { get; set; } = string.Empty;
    public int KycLevel { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
