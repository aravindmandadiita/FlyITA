namespace FlyITA.Web.Options;

public class AppSessionOptions
{
    public const string SectionName = "Session";
    public int TimeoutMinutes { get; set; } = 20;
    public int TimeoutWarningSeconds { get; set; } = 1080;
    public int TimeoutSeconds { get; set; } = 90;
}
