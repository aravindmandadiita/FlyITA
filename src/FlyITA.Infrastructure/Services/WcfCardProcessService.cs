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
        try
        {
            // SOAP call placeholder — actual implementation depends on WCF contract from WSDL
            _logger.LogDebug("ProcessPayment called for amount {Amount}", request.Amount);
            await Task.CompletedTask;
            return new PaymentResult { Success = false, ErrorMessage = "Service endpoint not configured" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process payment");
            return new PaymentResult { Success = false, ErrorMessage = $"Payment processing error: {ex.Message}" };
        }
    }

    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default)
    {
        try
        {
            // SOAP call placeholder — actual implementation depends on WCF contract from WSDL
            _logger.LogDebug("Refund called for transaction {TransactionId}, amount {Amount}", transactionId, amount);
            await Task.CompletedTask;
            return new PaymentResult { Success = false, ErrorMessage = "Service endpoint not configured" };
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
