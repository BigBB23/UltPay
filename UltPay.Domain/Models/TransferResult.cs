namespace UltPay.Domain.Models
{
    public class TransferResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ProviderReference { get; set; }
    }
}