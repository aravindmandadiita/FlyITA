namespace FlyITA.Web.Options;

public class LoginRedirectOptions
{
    public const string SectionName = "Authentication";
    public string MissingAuthenticationTarget { get; set; } = "Login";
    public string InvalidNavigationTarget { get; set; } = "Login";
}
