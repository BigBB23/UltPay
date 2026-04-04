using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Api.Controllers;
using UltPay.Api.Providers;
using UltPay.Api.Services;
using UltPay.Domain.Entities;
using UltPay.Domain.Models;
using UltPay.Infrastructure.Persistence;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UltPay.Api.Services
{
    public class TransferExecutionService : ITransferExecutionService
    {
        private readonly IWalletService _walletService;
        private readonly UltPayDbContext _context;
        private readonly ITransferProviderResolver _providerResolver;

        public TransferExecutionService(
            UltPayDbContext context,
            ITransferProviderResolver providerResolver,
            IWalletService walletService)
        {
            _context = context;
            _providerResolver = providerResolver;
            _walletService = walletService;
        }


        public async Task<string> ExecuteTransferAsync(Guid transferId)
        {
            var transfer = await _context.Transfers
                .FirstOrDefaultAsync(x => x.Id == transferId);

            if (transfer == null)
                return "NOT_FOUND";

            if (transfer.Status == "SUCCESS")
                return "ALREADY_PROCESSED";

            if (transfer.Status == "PROCESSING")
                return "ALREADY_PROCESSING";

            if (transfer.Status != "CREATED")
                return "INVALID_STATUS";

            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(x => x.Id == transfer.BeneficiaryId);

            if (beneficiary == null)
                return "BENEFICIARY_NOT_FOUND";

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(x =>
                    x.UserId == transfer.UserId &&
                    x.CurrencyCode != null &&
                    transfer.SourceCurrency != null &&
                    x.CurrencyCode.Trim().ToUpper() == transfer.SourceCurrency.Trim().ToUpper());

            if (wallet == null)
                return "WALLET_NOT_FOUND";

            if (wallet.AvailableBalance < transfer.SourceAmount)
                return "INSUFFICIENT_FUNDS";

            // Step 1: reserve funds
            var reserveBalanceBeforeAvailable = wallet.AvailableBalance;
            var reserveBalanceBeforeReserved = wallet.ReservedBalance;

            wallet.AvailableBalance -= transfer.SourceAmount;
            wallet.ReservedBalance += transfer.SourceAmount;
            wallet.UpdatedAtUtc = DateTime.UtcNow;

            _context.WalletTransactions.Add(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.Id,
                UserId = wallet.UserId,
                CurrencyCode = wallet.CurrencyCode,
                Amount = transfer.SourceAmount,
                Type = "RESERVE",
                ReferenceType = "TRANSFER",
                ReferenceId = transfer.Id,
                BalanceBefore = reserveBalanceBeforeAvailable,
                BalanceAfter = wallet.AvailableBalance,
                Narration = $"Funds reserved for transfer: {transfer.Id}",
                CreatedAtUtc = DateTime.UtcNow
            });

            transfer.Status = "PROCESSING";
            transfer.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Step 2: call provider
            var provider = _providerResolver.Resolve(
                transfer.DestinationCurrency,
                beneficiary.PayoutMethod
            );

            var providerResult = await provider.SendTransferAsync(transfer, beneficiary);

            // Step 3A: provider failed -> release funds
            if (!providerResult.Success)
            {
                var releaseBalanceBeforeAvailable = wallet.AvailableBalance;

                wallet.ReservedBalance -= transfer.SourceAmount;
                wallet.AvailableBalance += transfer.SourceAmount;
                wallet.UpdatedAtUtc = DateTime.UtcNow;

                _context.WalletTransactions.Add(new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    WalletId = wallet.Id,
                    UserId = wallet.UserId,
                    CurrencyCode = wallet.CurrencyCode,
                    Amount = transfer.SourceAmount,
                    Type = "RELEASE",
                    ReferenceType = "TRANSFER",
                    ReferenceId = transfer.Id,
                    BalanceBefore = releaseBalanceBeforeAvailable,
                    BalanceAfter = wallet.AvailableBalance,
                    Narration = $"Funds released for failed transfer: {transfer.Id}",
                    CreatedAtUtc = DateTime.UtcNow
                });

                transfer.Status = "FAILED";
                transfer.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return "FAILED";
            }

            // Step 3B: provider success -> finalize debit from reserved
            var debitBalanceBeforeAvailable = wallet.AvailableBalance;

            wallet.ReservedBalance -= transfer.SourceAmount;
            wallet.UpdatedAtUtc = DateTime.UtcNow;

            transfer.Status = "PROCESSING";
            transfer.ProviderReference = providerResult.ProviderReference;
            transfer.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return "PROCESSING";

        }



    }
}