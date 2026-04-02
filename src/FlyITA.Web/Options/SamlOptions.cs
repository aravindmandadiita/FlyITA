using Microsoft.AspNetCore.Authentication;

namespace FlyITA.Web.Options;

public class SamlOptions : AuthenticationSchemeOptions
{
    public const string SectionName = "Saml";
    public bool Enabled { get; set; } = false;
    public string IdpSsoUrl { get; set; } = string.Empty;
    public string IdpSloUrl { get; set; } = string.Empty;
    public string IdpStagingSsoUrl { get; set; } = string.Empty;
    public string IdpCertificateFile { get; set; } = string.Empty;
    public string SpEntityId { get; set; } = string.Empty;
    public string AssertionConsumerServiceUrl { get; set; } = string.Empty;
    public string DirectLoginUrl { get; set; } = string.Empty;
    public string SeamlessLoginUrl { get; set; } = string.Empty;
    public bool DebugMode { get; set; } = false;
}
