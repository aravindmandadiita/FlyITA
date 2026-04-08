using System.Net.Http.Json;
using System.Text.Json;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using Microsoft.Extensions.Logging;

namespace FlyITA.Infrastructure.Services;

public class LegacyApiCardProcessService : ICardProcessService
{
    private readonly HttpClient _http;
    private readonly ILogger<LegacyApiCardProcessService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public LegacyApiCardProcessService(HttpClient http, ILogger<LegacyApiCardProcessService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Submitting ACH payment for participant {ParticipantId}, amount {Amount} {Currency}",
            request.ParticipantId, request.Amount, request.Currency);

        try
        {
            using var response = await _http.PostAsJsonAsync("api/payments/ach", request, JsonOptions, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("ACH payment failed with HTTP {StatusCode}: {Body}",
                    (int)response.StatusCode, body);

                return TryDeserializeError(body) ?? new PaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Payment service returned HTTP {(int)response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<PaymentResult>(JsonOptions, ct);
            if (result is null)
            {
                _logger.LogError("ACH payment response deserialized to null");
                return new PaymentResult { Success = false, ErrorMessage = "Invalid response from payment service" };
            }

            _logger.LogInformation("ACH payment completed: Success={Success}, TransactionId={TransactionId}",
                result.Success, result.TransactionId);

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize ACH payment response");
            return new PaymentResult { Success = false, ErrorMessage = "Invalid response from payment service" };
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogError(ex, "ACH payment request timed out");
            return new PaymentResult { Success = false, ErrorMessage = "Payment service request timed out" };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "ACH payment request failed due to network error");
            return new PaymentResult { Success = false, ErrorMessage = $"Payment service unavailable: {ex.Message}" };
        }
    }

    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default)
    {
        _logger.LogInformation("Submitting refund for transaction {TransactionId}, amount {Amount}",
            transactionId, amount);

        try
        {
            var payload = new { transactionId, amount };
            using var response = await _http.PostAsJsonAsync("api/payments/refund", payload, JsonOptions, ct);

            if (response.StatusCode == System.Net.HttpStatusCode.NotImplemented)
            {
                _logger.LogWarning("Refund not yet implemented in sidecar");
                return new PaymentResult { Success = false, ErrorMessage = "Refund not yet available" };
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Refund failed with HTTP {StatusCode}: {Body}",
                    (int)response.StatusCode, body);

                return TryDeserializeError(body) ?? new PaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Refund service returned HTTP {(int)response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<PaymentResult>(JsonOptions, ct);
            return result ?? new PaymentResult { Success = false, ErrorMessage = "Invalid response from refund service" };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize refund response");
            return new PaymentResult { Success = false, ErrorMessage = "Invalid response from refund service" };
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogError(ex, "Refund request timed out");
            return new PaymentResult { Success = false, ErrorMessage = "Refund service request timed out" };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Refund request failed due to network error");
            return new PaymentResult { Success = false, ErrorMessage = $"Refund service unavailable: {ex.Message}" };
        }
    }

    private static PaymentResult? TryDeserializeError(string body)
    {
        try
        {
            return JsonSerializer.Deserialize<PaymentResult>(body, JsonOptions);
        }
        catch
        {
            return null;
        }
    }
}
