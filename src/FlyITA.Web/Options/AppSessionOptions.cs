namespace FlyITA.Web.Options;

public class AppSessionOptions
{
    public const string SectionName = "Session";
    public int TimeoutMinutes { get; set; } = 20;
}
