namespace FlyITA.Core.Interfaces;

public interface IPageConfigurationService
{
    Dictionary<string, object> ReadPageConfigSettings(string callingPage, string? programNumber = null);
    bool PersonMatchingEnabled(string? programNumber = null);
}
