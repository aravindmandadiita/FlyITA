using System.ServiceModel;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using Microsoft.Extensions.Logging;

namespace FlyITA.Infrastructure.Services;

public class WcfCardProcessService : ICardProcessService
{
    private readonly string _endpointUrl;
    private readonly int _timeoutSeconds;
    private readonly ILogger<WcfCardProcessService> _logger;

    public WcfCardProcessService(string endpointUrl, int timeoutSeconds, ILogger<WcfCardProcessService> logger)
    {
        _endpointUrl = endpointUrl;
        _timeoutSeconds = timeoutSeconds;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("ProcessPayment skipped — endpoint URL not configured");
            return new PaymentResult { Success = false, ErrorMessage = "Service endpoint not configured" };
        }

        try
        {
            var binding = CreateBinding();
            var endpoint = new EndpointAddress(_endpointUrl);

            _logger.LogWarning("SOAP client contract not yet wired for endpoint {EndpointUrl}", _endpointUrl);
            throw new NotSupportedException(
                $"SOAP client contract implementation not yet added for endpoint '{_endpointUrl}'");
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment");
            return new PaymentResult { Success = false, ErrorMessage = $"Payment processing error: {ex.Message}" };
        }
    }

    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("Refund skipped — endpoint URL not configured");
            return new PaymentResult { Success = false, ErrorMessage = "Service endpoint not configured" };
        }

        try
        {
            var binding = CreateBinding();
            var endpoint = new EndpointAddress(_endpointUrl);

            _logger.LogWarning("SOAP client contract not yet wired for endpoint {EndpointUrl}", _endpointUrl);
            throw new NotSupportedException(
                $"SOAP client contract implementation not yet added for endpoint '{_endpointUrl}'");
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process refund for transaction {TransactionId}", transactionId);
            return new PaymentResult { Success = false, ErrorMessage = $"Refund processing error: {ex.Message}" };
        }
    }

    internal BasicHttpsBinding CreateBinding()
    {
        return new BasicHttpsBinding
        {
            Security = { Mode = BasicHttpsSecurityMode.Transport },
            SendTimeout = TimeSpan.FromSeconds(_timeoutSeconds),
            ReceiveTimeout = TimeSpan.FromSeconds(_timeoutSeconds)
        };
    }
}
