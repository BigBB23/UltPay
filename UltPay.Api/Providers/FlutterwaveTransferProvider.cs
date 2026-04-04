using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UltPay.Domain.Entities;
using UltPay.Api.Providers.Flutterwave;

namespace UltPay.Api.Providers
{
    public class FlutterwaveTransferProvider : ITransferProvider
    {
        private readonly HttpClient _httpClient;
        private readonly UltPay.Infrastructure.Providers.Flutterwave.FlutterwaveOptions _options;
        private readonly ILogger<FlutterwaveTransferProvider> _logger;

        public string Name => "Flutterwave";

        public FlutterwaveTransferProvider(
            HttpClient httpClient,
            IOptions<UltPay.Infrastructure.Providers.Flutterwave.FlutterwaveOptions> options,
            ILogger<FlutterwaveTransferProvider> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<TransferProviderResult> SendTransferAsync(
            Transfer transfer,
            Beneficiary beneficiary)
        {
            if (string.IsNullOrWhiteSpace(beneficiary.BankCode) ||
                string.IsNullOrWhiteSpace(beneficiary.AccountNumber))
            {
                return new TransferProviderResult
                {
                    Success = false,
                    ProviderName = Name,
                    Status = "FAILED",
                    ErrorMessage = "BankCode and AccountNumber are required for bank transfer."
                };
            }

            var request = new FlutterwaveTransferRequest
            {
                account_bank = beneficiary.BankCode!,
                account_number = beneficiary.AccountNumber!,
                amount = transfer.DestinationAmount,
                narration = $"UltPay transfer {transfer.Id}",
                currency = transfer.DestinationCurrency,
                reference = transfer.CorrelationId,
                beneficiary_name = beneficiary.FullName,
                debit_currency = transfer.SourceCurrency
            };

            var json = JsonSerializer.Serialize(request);

            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_options.BaseUrl}/transfers");

            message.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.AccessToken);

            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(message);
            var body = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Flutterwave transfer response: {Body}", body);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Flutterwave FAILED response: {Body}", body);

                return new TransferProviderResult
                {
                    Success = false,
                    ProviderName = Name,
                    Status = "FAILED",
                    ErrorMessage = body
                };
            }

            var flutterwaveResponse = JsonSerializer.Deserialize<FlutterwaveTransferResponse>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var providerReference =
                flutterwaveResponse?.data?.reference ?? transfer.CorrelationId;

            var providerStatus =
                flutterwaveResponse?.data?.status ?? "PROCESSING";

            _logger.LogInformation("Flutterwave RAW response: {Body}", body);

            return new TransferProviderResult
            {
                Success = true,
                ProviderName = Name,
                ProviderReference = providerReference,
                Status = providerStatus.ToUpperInvariant()
            };
        }
    }
}
