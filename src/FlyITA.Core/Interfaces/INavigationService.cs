namespace FlyITA.Core.Interfaces;

public interface INavigationService
{
    string GetNextPage(string navOption = "default");
    string GetNextPageByPage(string currentPage, string navOption = "default");
    void SetNextPage(string nextPage);
    string Navigate(string nextPage);
    string GetCurrentPage();
    Dictionary<string, string> ReadNavConfigValues(string section);
}
