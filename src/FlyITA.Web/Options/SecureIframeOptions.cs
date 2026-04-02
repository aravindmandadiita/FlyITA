namespace FlyITA.Web.Options;

public class SecureIframeOptions
{
    public const string SectionName = "SecureIframe";
    public string CaptureUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
}
