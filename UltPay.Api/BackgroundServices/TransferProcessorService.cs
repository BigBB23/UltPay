using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using UltPay.Api.BackgroundServices;
using UltPay.Api.Services;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UltPay.Api.BackgroundServices
{
    public class TransferProcessorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransferProcessorService> _logger;

        public TransferProcessorService(
            IServiceScopeFactory scopeFactory,
            ILogger<TransferProcessorService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TransferProcessorService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<UltPayDbContext>();
                    var transferExecutionService = scope.ServiceProvider.GetRequiredService<ITransferExecutionService>();

                    var pendingTransfers = await context.Transfers
                        .Where(x => x.Status == "CREATED")
                        .OrderBy(x => x.CreatedAtUtc)
                        .Select(x => x.Id)
                        .ToListAsync(stoppingToken);

                    foreach (var transferId in pendingTransfers)
                    {
                        try
                        {
                            _logger.LogInformation("Executing transfer {TransferId}", transferId);
                            await transferExecutionService.ExecuteTransferAsync(transferId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error executing transfer {TransferId}", transferId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inside transfer processor loop.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
