using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;

namespace FlyITA.Core.Services;

public class NavigationService : INavigationService
{
    private readonly NavigationOptions _options;
    private readonly IContextManager _context;

    public NavigationService(IOptions<NavigationOptions> navigationOptions, IContextManager context)
    {
        _options = navigationOptions.Value;
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
        _context.NextPage = nextPage;
    }

    public string Navigate(string nextPage)
    {
        return nextPage;
    }

    public string GetCurrentPage()
    {
        return _context.NextPage ?? string.Empty;
    }

    public Dictionary<string, string> ReadNavConfigValues(string section)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Search registration flow steps by name
        var step = _options.RegistrationFlow.FirstOrDefault(
            s => s.Name.Equals(section, StringComparison.OrdinalIgnoreCase));
        if (step != null)
        {
            result["default"] = step.Url;
        }

        // Search links by name
        foreach (var link in _options.Links.Where(
            l => l.Name.Equals(section, StringComparison.OrdinalIgnoreCase)))
        {
            result["url"] = link.Url;
            if (link.ShowLeft) result["show_left"] = "true";
            if (link.ShowFooter) result["show_footer"] = "true";
            if (link.ShowTop) result["show_top"] = "true";
            result["registered"] = link.Registered;
            if (link.Role != null) result["role"] = link.Role;
            if (link.Type != null) result["type"] = link.Type;
            if (link.CssClass != null) result["class"] = link.CssClass;
        }

        // Search registration levels
        var level = _options.RegistrationLevels.FirstOrDefault(
            l => l.Level.ToString() == section);
        if (level != null)
        {
            result["required_level"] = level.RequiredLevel.ToString();
        }

        return result;
    }
}
