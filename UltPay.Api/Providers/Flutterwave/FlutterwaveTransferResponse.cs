namespace UltPay.Api.Providers.Flutterwave
{
    public class FlutterwaveTransferResponse
    {
        public string status { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public FlutterwaveTransferResponseData? data { get; set; }
    }

    public class FlutterwaveTransferResponseData
    {
        public int id { get; set; }
        public string reference { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string complete_message { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
    }
}