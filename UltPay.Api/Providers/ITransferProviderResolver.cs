namespace UltPay.Api.Providers
{
    public interface ITransferProviderResolver
    {
        ITransferProvider Resolve(string destinationCurrency, string payoutMethod);
    }
}