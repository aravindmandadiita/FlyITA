using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Services;

namespace FlyITA.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddFlyITACore(this IServiceCollection services)
    {
        // Scoped services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IPageConfigurationService, PageConfigurationService>();
        services.AddScoped<IAccommodationsService, AccommodationsService>();
        services.AddScoped<IFeesDisplayService, FeesDisplayService>();
        services.AddScoped<IClosedPageService, ClosedPageService>();
        services.AddScoped<ICustomFieldValidationService, CustomFieldValidationService>();

        // Singleton (AddMemoryCache ensures IMemoryCache is available)
        services.AddMemoryCache();
        services.AddSingleton<ICustomFieldsCache, CustomFieldsCache>();

        // Data access abstractions — TryAdd so Infrastructure can override with real implementations
        services.TryAddScoped<IPCentralDataAccess, NullPCentralDataAccess>();
        services.TryAddScoped<IDatabaseAccess, NullDatabaseAccess>();

        // Pluggable services — TryAdd so Web can override with real implementations
        services.TryAddSingleton<IEnvironmentService, NullEnvironmentService>();
        services.TryAddScoped<ISmtpClient, NullSmtpClient>();
        services.TryAddScoped<IEmailTemplateLoader, NullEmailTemplateLoader>();

        // External service abstractions — TryAdd so Infrastructure can override
        services.TryAddScoped<ICaptchaService, NullCaptchaService>();
        services.TryAddScoped<ICardTokenService, NullCardTokenService>();
        services.TryAddScoped<IPerformanceCentralClient, NullPerformanceCentralClient>();
        services.TryAddScoped<ICardProcessService, NullCardProcessService>();

        return services;
    }
}

/// <summary>No-op implementation until Infrastructure provides a real one.</summary>
internal class NullPCentralDataAccess : IPCentralDataAccess
{
    public Task<Dictionary<string, object?>?> GetParticipantByIdAsync(int participantId) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<Dictionary<string, object?>?> GetPersonByIdAsync(int personId) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<Dictionary<string, object?>?> GetPartyByParticipantIdAsync(int participantId) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<Dictionary<string, object?>?> GetProgramByIdAsync(int programId) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<List<Dictionary<string, object?>>> GetCustomFieldValuesAsync(int participantId) => Task.FromResult(new List<Dictionary<string, object?>>());
    public Task SaveCustomFieldValueAsync(int participantId, int customFieldId, string value, int possibleValueId) => Task.CompletedTask;
    public Task<Dictionary<string, object?>?> GetAccommodationDetailsAsync(int participantId) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<List<Dictionary<string, object?>>> GetAccommodationListAsync(int participantId) => Task.FromResult(new List<Dictionary<string, object?>>());
    public Task SaveAccommodationRecordAsync(int participantId, Dictionary<string, object?> data) => Task.CompletedTask;
    public Task DeleteAccommodationRecordAsync(int participantId, string recordType) => Task.CompletedTask;
    public Task<string?> GetEmailTemplateAsync(string templateName, int programId) => Task.FromResult<string?>(null);
    public Task<string?> GetEmailBodyAsync(string templateKey, int programId) => Task.FromResult<string?>(null);
    public Task<List<Dictionary<string, object?>>> GetContactNumbersAsync(int personId) => Task.FromResult(new List<Dictionary<string, object?>>());
    public Task<Dictionary<string, object?>?> GetPageConfigurationAsync(string pageName, string programNumber) => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<Dictionary<string, object?>?> GetTransportationDetailsAsync(int participantId) => Task.FromResult<Dictionary<string, object?>?>(null);
}

internal class NullDatabaseAccess : IDatabaseAccess
{
    public Dictionary<string, object?>? ExecuteStoredProcedure(string spName, Dictionary<string, object?> parameters) => null;
    public int ExecuteNonQuery(string spName, Dictionary<string, object?> parameters) => 0;
    public List<Dictionary<string, object?>> ExecuteStoredProcedureList(string spName, Dictionary<string, object?> parameters) => new();
}

internal class NullEnvironmentService : IEnvironmentService
{
    public string Role => "CDT";
    public bool IsClientFacing => false;
    public string GetConnectionStringName() => "Default";
}

internal class NullEmailTemplateLoader : IEmailTemplateLoader
{
    public Task<string?> LoadTemplateAsync(string templateName, CancellationToken ct = default)
        => Task.FromResult<string?>(null);
}

internal class NullSmtpClient : ISmtpClient
{
    public Task SendAsync(System.Net.Mail.MailMessage message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

internal class NullCaptchaService : ICaptchaService
{
    public Task<Models.CaptchaValidationResult> ValidateAsync(string token, string? remoteIp = null, CancellationToken ct = default)
        => Task.FromResult(new Models.CaptchaValidationResult { IsValid = true, Score = 1.0 });
}

internal class NullCardTokenService : ICardTokenService
{
    public Task<string?> TokenizeCardAsync(string cardNumber, string expiryDate, CancellationToken ct = default)
        => Task.FromResult<string?>(null);
    public Task<Models.CardInfo?> GetCardInfoByTokenAsync(string token, CancellationToken ct = default)
        => Task.FromResult<Models.CardInfo?>(null);
}

internal class NullPerformanceCentralClient : IPerformanceCentralClient
{
    public Task<Dictionary<string, object?>?> GetBookingAsync(int participantId, CancellationToken ct = default)
        => Task.FromResult<Dictionary<string, object?>?>(null);
    public Task<bool> UpdateBookingAsync(int participantId, Dictionary<string, object?> data, CancellationToken ct = default)
        => Task.FromResult(false);
}

internal class NullCardProcessService : ICardProcessService
{
    public Task<Models.PaymentResult> ProcessPaymentAsync(Models.PaymentRequest request, CancellationToken ct = default)
        => Task.FromResult(new Models.PaymentResult { Success = false, ErrorMessage = "Not configured" });
    public Task<Models.PaymentResult> RefundAsync(string transactionId, decimal amount, CancellationToken ct = default)
        => Task.FromResult(new Models.PaymentResult { Success = false, ErrorMessage = "Not configured" });
}
