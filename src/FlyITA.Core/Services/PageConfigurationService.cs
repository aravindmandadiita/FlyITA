using Microsoft.Extensions.Configuration;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Services;

public class PageConfigurationService : IPageConfigurationService
{
    private readonly IDatabaseAccess _db;
    private readonly IConfiguration _configuration;

    public PageConfigurationService(IDatabaseAccess db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public Dictionary<string, object> ReadPageConfigSettings(string callingPage, string? programNumber = null)
    {
        programNumber ??= GetITAProgNbr();

        var parameters = new Dictionary<string, object?>
        {
            ["@PageName"] = callingPage,
            ["@ITAProgNbr"] = programNumber
        };

        var row = _db.ExecuteStoredProcedure("spWRASelPageConfiguration", parameters);

        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        if (row != null)
        {
            foreach (var kvp in row)
            {
                if (kvp.Value != null)
                    result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }

    public bool PersonMatchingEnabled(string? programNumber = null)
    {
        programNumber ??= GetITAProgNbr();
        var settings = ReadPageConfigSettings("selfenroll", programNumber);
        return settings.TryGetValue("personmatching", out var value) && value is true or "true" or "True";
    }

    private string GetITAProgNbr(string? programNumber = null)
    {
        if (!string.IsNullOrEmpty(programNumber))
            return programNumber;

        return _configuration["ITAProgNbr"] ?? string.Empty;
    }
}
