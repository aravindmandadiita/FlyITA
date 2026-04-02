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
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("TokenizeCard skipped — endpoint URL not configured");
            return null;
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
            _logger.LogError(ex, "Failed to tokenize card");
            return null;
        }
    }

    public async Task<CardInfo?> GetCardInfoByTokenAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("GetCardInfoByToken skipped — endpoint URL not configured");
            return null;
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
