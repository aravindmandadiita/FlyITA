namespace FlyITA.Web.Options;

public class SecurityOptions
{
    public const string SectionName = "Security";
    public string ContentSecurityPolicy { get; set; } = string.Empty;
    public bool RequireHttps { get; set; } = true;
}
