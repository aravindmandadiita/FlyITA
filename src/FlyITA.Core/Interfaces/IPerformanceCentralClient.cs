namespace FlyITA.Core.Interfaces;

public interface IPerformanceCentralClient
{
    Task<Dictionary<string, object?>?> GetBookingAsync(int participantId, CancellationToken ct = default);
    Task<bool> UpdateBookingAsync(int participantId, Dictionary<string, object?> data, CancellationToken ct = default);
}
