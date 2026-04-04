using System;
using System.Collections.Generic;
using System.Linq;

namespace UltPay.Api.Providers
{
    public class TransferProviderResolver : ITransferProviderResolver
    {
        private readonly IEnumerable<ITransferProvider> _providers;

        public TransferProviderResolver(IEnumerable<ITransferProvider> providers)
        {
            _providers = providers;
        }

        public ITransferProvider Resolve(string destinationCurrency, string payoutMethod)
        {
            // For now always use Flutterwave.
            // Later:
            // - KES mobile money → M-Pesa provider
            // - ETB mobile money → Telebirr provider
            // - NGN bank → Paystack / Flutterwave
            // etc.

            var provider = _providers.FirstOrDefault(x => x.Name == "Flutterwave");

            if (provider == null)
                throw new InvalidOperationException("No transfer provider configured.");

            return provider;
        }
    }
}