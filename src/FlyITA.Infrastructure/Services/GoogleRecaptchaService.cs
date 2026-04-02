using System.Text.Json;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Core.Options;

namespace FlyITA.Infrastructure.Services;

public class GoogleRecaptchaService : ICaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly RecaptchaOptions _options;

    public GoogleRecaptchaService(HttpClient httpClient, IOptions<RecaptchaOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<CaptchaValidationResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken ct = default)
    {
        if (!_options.Enabled)
            return new CaptchaValidationResult { IsValid = true, Score = 1.0 };

        try
        {
            var parameters = new Dictionary<string, string>
            {
                ["secret"] = _options.SecretKey,
                ["response"] = token
            };
            if (remoteIp != null)
                parameters["remoteip"] = remoteIp;

            var response = await _httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(parameters),
                ct);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var success = root.GetProperty("success").GetBoolean();
            var score = root.TryGetProperty("score", out var scoreProp) ? scoreProp.GetDouble() : 0.0;

            return new CaptchaValidationResult
            {
                IsValid = success && score >= _options.MinimumScore,
                Score = score,
                ErrorMessage = !success ? "reCAPTCHA verification failed" : score < _options.MinimumScore ? $"Score {score} below minimum {_options.MinimumScore}" : null
            };
        }
        catch (Exception ex)
        {
            return new CaptchaValidationResult
            {
                IsValid = false,
                Score = 0,
                ErrorMessage = $"reCAPTCHA validation error: {ex.Message}"
            };
        }
    }
}
