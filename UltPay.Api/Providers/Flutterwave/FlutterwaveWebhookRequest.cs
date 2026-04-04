namespace UltPay.Api.Providers.Flutterwave
{
    public class FlutterwaveWebhookRequest
    {
        public string? status { get; set; }
        public string? tx_ref { get; set; }
        public string? transaction_id { get; set; }
        public FlutterwaveWebhookData? data { get; set; }
    }

    public class FlutterwaveWebhookData
    {
        public string? id { get; set; }
        public string? tx_ref { get; set; }
        public string? status { get; set; }
        public string? reference { get; set; }
    }
}