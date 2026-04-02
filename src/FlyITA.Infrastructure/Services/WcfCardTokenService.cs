using System.ServiceModel;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using Microsoft.Extensions.Logging;

namespace FlyITA.Infrastructure.Services;

public class WcfCardTokenService : ICardTokenService
{
    private readonly string _endpointUrl;
    private readonly int _timeoutSeconds;
    private readonly ILogger<WcfCardTokenService> _logger;

    public WcfCardTokenService(string endpointUrl, int timeoutSeconds, ILogger<WcfCardTokenService> logger)
    {
        _endpointUrl = endpointUrl;
        _timeoutSeconds = timeoutSeconds;
        _logger = logger;
    }

    public async Task<string?> TokenizeCardAsync(string cardNumber, string expiryDate, CancellationToken ct = default)
    {
        try
        {
            // SOAP call placeholder — actual implementation depends on WCF contract from WSDL
            _logger.LogDebug("TokenizeCard called for card ending {Last4}", cardNumber.Length >= 4 ? cardNumber[^4..] : "****");
            await Task.CompletedTask;
            return null; // Real implementation will call SOAP endpoint
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to tokenize card");
            return null;
        }
    }

    public async Task<CardInfo?> GetCardInfoByTokenAsync(string token, CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("GetCardInfoByToken called for token {Token}", token);
            await Task.CompletedTask;
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get card info by token");
            return null;
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
