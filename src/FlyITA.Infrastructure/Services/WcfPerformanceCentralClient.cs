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
        try
        {
            // SOAP call placeholder — actual implementation depends on WCF contract from WSDL
            _logger.LogDebug("GetBooking called for participant {ParticipantId}", participantId);
            await Task.CompletedTask;
            return null; // Real implementation will call SOAP endpoint
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get booking for participant {ParticipantId}", participantId);
            return null;
        }
    }

    public async Task<bool> UpdateBookingAsync(int participantId, Dictionary<string, object?> data, CancellationToken ct = default)
    {
        try
        {
            // SOAP call placeholder — actual implementation depends on WCF contract from WSDL
            _logger.LogDebug("UpdateBooking called for participant {ParticipantId}", participantId);
            await Task.CompletedTask;
            return false; // Real implementation will call SOAP endpoint
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
