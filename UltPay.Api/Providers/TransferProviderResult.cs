namespace UltPay.Api.Providers
{
    public class TransferProviderResult
    {
        public bool Success { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string ProviderReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

    }
}