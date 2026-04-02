using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface ICaptchaService
{
    Task<CaptchaValidationResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken ct = default);
}
