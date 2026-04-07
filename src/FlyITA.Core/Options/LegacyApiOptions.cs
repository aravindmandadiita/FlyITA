namespace FlyITA.Core.Options;

public class LegacyApiOptions
{
    public const string SectionName = "LegacyApi";
    public string BaseUrl { get; set; } = "http://localhost:5100";
    public int TimeoutSeconds { get; set; } = 30;
}
