using System.ServiceModel;
using FlyITA.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlyITA.Infrastructure.Services;

public class WcfPerformanceCentralClient : IPerformanceCentralClient
{
    private readonly string _endpointUrl;
    private readonly int _timeoutSeconds;
    private readonly ILogger<WcfPerformanceCentralClient> _logger;

    public WcfPerformanceCentralClient(string endpointUrl, int timeoutSeconds, ILogger<WcfPerformanceCentralClient> logger)
    {
        _endpointUrl = endpointUrl;
        _timeoutSeconds = timeoutSeconds;
        _logger = logger;
    }

    public async Task<Dictionary<string, object?>?> GetBookingAsync(int participantId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("GetBooking skipped — endpoint URL not configured");
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
            _logger.LogError(ex, "Failed to get booking for participant {ParticipantId}", participantId);
            return null;
        }
    }

    public async Task<bool> UpdateBookingAsync(int participantId, Dictionary<string, object?> data, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_endpointUrl))
        {
            _logger.LogDebug("UpdateBooking skipped — endpoint URL not configured");
            return false;
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
            _logger.LogError(ex, "Failed to update booking for participant {ParticipantId}", participantId);
            return false;
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
