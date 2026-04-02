namespace FlyITA.Core.Interfaces;

public interface INavigationService
{
    string GetNextPage(string navOption = "default");
    string GetNextPageByPage(string currentPage, string navOption = "default");
    void SetNextPage(string nextPage);
    string Navigate(string nextPage);
    string GetCurrentPage();
    Dictionary<string, string> ReadNavConfigValues(string section);
    string ReadCustomConfigValue(string section, string key = "value", string notFound = "");
    Dictionary<string, string> ReadCustomConfig(string section);
    string GetEnvironment(string? role = null);
    string GetRole();
    bool IsClientFacingEnv();
}
