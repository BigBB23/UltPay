

namespace UltPay.Api.Providers.Flutterwave
{
    public class FlutterwaveTransferRequest
    {
        public string account_bank { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public decimal amount { get; set; }
        public string narration { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string beneficiary_name { get; set; } = string.Empty;

        
        public string? debit_currency { get; set; }
    }
}