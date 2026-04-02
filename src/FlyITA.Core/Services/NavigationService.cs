using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Services;

public class NavigationService : INavigationService
{
    private readonly IConfiguration _configuration;
    private readonly IContextManager _context;

    public NavigationService(IConfiguration configuration, IContextManager context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string GetNextPage(string navOption = "default")
    {
        var currentPage = GetCurrentPage();
        return GetNextPageByPage(currentPage, navOption);
    }

    public string GetNextPageByPage(string currentPage, string navOption = "default")
    {
        var navConfig = ReadNavConfigValues(currentPage);
        if (navConfig.TryGetValue(navOption, out var target))
            return target;
        if (navConfig.TryGetValue("default", out var defaultTarget))
            return defaultTarget;
        return string.Empty;
    }

    public void SetNextPage(string nextPage)
    {
        _context.CardToken = nextPage; // Uses session to store next page target
    }

    public string Navigate(string nextPage)
    {
        return nextPage;
    }

    public string GetCurrentPage()
    {
        // In legacy this reads from HttpContext.Current.Request.Path
        // In Core, this will be set by the caller (Razor Page)
        return _configuration["CurrentPage"] ?? string.Empty;
    }

    public Dictionary<string, string> ReadNavConfigValues(string section)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var configPath = _configuration["NavigationConfigPath"];

        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            return result;

        try
        {
            var doc = XDocument.Load(configPath);
            var node = doc.Descendants(section).FirstOrDefault();
            if (node == null) return result;

            foreach (var attr in node.Attributes())
            {
                result[attr.Name.LocalName] = attr.Value;
            }

            foreach (var child in node.Elements())
            {
                var key = child.Name.LocalName;
                var value = child.Attribute("value")?.Value ?? child.Value;
                result[key] = value;
            }
        }
        catch
        {
            // Config read failure — return empty
        }

        return result;
    }

    public string ReadCustomConfigValue(string section, string key = "value", string notFound = "")
    {
        var config = ReadCustomConfig(section);
        return config.TryGetValue(key, out var value) ? value : notFound;
    }

    public Dictionary<string, string> ReadCustomConfig(string section)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var configPath = _configuration["WebRegistrationConfigPath"];

        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            return result;

        try
        {
            var doc = XDocument.Load(configPath);
            var node = doc.Descendants(section).FirstOrDefault();
            if (node == null) return result;

            foreach (var attr in node.Attributes())
            {
                result[attr.Name.LocalName] = attr.Value;
            }
        }
        catch
        {
            // Config read failure
        }

        return result;
    }

    public string GetEnvironment(string? role = null)
    {
        role ??= GetRole();
        var envMappings = _configuration.GetSection("EnvironmentMappings");
        return envMappings[role] ?? "Development";
    }

    public string GetRole()
    {
        return _configuration["Role"] ?? "CDT";
    }

    public bool IsClientFacingEnv()
    {
        var env = GetEnvironment();
        return env == "CA" || env == "PR";
    }
}
