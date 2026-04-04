using System;
using System.Threading.Tasks;

namespace UltPay.Api.Services
{
    public interface ITransferExecutionService
    {
        Task<string> ExecuteTransferAsync(Guid transferId);
    }
}