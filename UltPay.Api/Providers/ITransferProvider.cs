using System.Threading.Tasks;
using UltPay.Domain.Entities;

namespace UltPay.Api.Providers
{
    public interface ITransferProvider
    {
        string Name { get; }

        Task<TransferProviderResult> SendTransferAsync(
            Transfer transfer,
            Beneficiary beneficiary);
    }
}