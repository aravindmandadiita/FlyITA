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

        return services;
    }
}

/// <summary>No-op implementation until Infrastructure provides a real one.</summary>
internal class NullPCentralDataAccess : IPCentralDataAccess
{
    public Dictionary<string, object?>? GetParticipantById(int participantId) => null;
    public Dictionary<string, object?>? GetPersonById(int personId) => null;
    public Dictionary<string, object?>? GetPartyByParticipantId(int participantId) => null;
    public Dictionary<string, object?>? GetProgramById(int programId) => null;
    public List<Dictionary<string, object?>> GetCustomFieldValues(int participantId) => new();
    public void SaveCustomFieldValue(int participantId, int customFieldId, string value, int possibleValueId) { }
    public Dictionary<string, object?>? GetAccommodationDetails(int participantId) => null;
    public List<Dictionary<string, object?>> GetAccommodationList(int participantId) => new();
    public void SaveAccommodationRecord(int participantId, Dictionary<string, object?> data) { }
    public void DeleteAccommodationRecord(int participantId, string recordType) { }
    public string? GetEmailTemplate(string templateName, int programId) => null;
    public string? GetEmailBody(string templateKey, int programId) => null;
    public List<Dictionary<string, object?>> GetContactNumbers(int personId) => new();
    public Dictionary<string, object?>? GetPageConfiguration(string pageName, string programNumber) => null;
    public Dictionary<string, object?>? GetTransportationDetails(int participantId) => null;
}

internal class NullDatabaseAccess : IDatabaseAccess
{
    public Dictionary<string, object?>? ExecuteStoredProcedure(string spName, Dictionary<string, object?> parameters) => null;
    public int ExecuteNonQuery(string spName, Dictionary<string, object?> parameters) => 0;
}

internal class NullEnvironmentService : IEnvironmentService
{
    public string Role => "CDT";
    public bool IsClientFacing => false;
    public string GetConnectionStringName() => "Default";
}

internal class NullSmtpClient : ISmtpClient
{
    public Task SendAsync(System.Net.Mail.MailMessage message, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
