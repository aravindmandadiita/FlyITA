using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using FlyITA.Core.Options;
using FlyITA.Web.Options;

namespace FlyITA.Web.Tests.Options;

public class OptionsBindingTests
{
    private static T BindOptions<T>(string sectionName, Dictionary<string, string?> config) where T : class, new()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();

        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    [Fact]
    public void ProgramOptions_BindsFromSection()
    {
        var opts = BindOptions<ProgramOptions>("Program", new()
        {
            ["Program:ProgramID"] = "42",
            ["Program:ITAProgNbr"] = "ABC123",
            ["Program:TrackingCode"] = "TRK"
        });

        Assert.Equal(42, opts.ProgramID);
        Assert.Equal("ABC123", opts.ITAProgNbr);
        Assert.Equal("TRK", opts.TrackingCode);
    }

    [Fact]
    public void EnvironmentOptions_BindsFromSection()
    {
        var opts = BindOptions<EnvironmentOptions>("Environment", new()
        {
            ["Environment:Role"] = "PR",
            ["Environment:IsClientFacing"] = "true",
            ["Environment:ConnectionStringName"] = "Production"
        });

        Assert.Equal("PR", opts.Role);
        Assert.True(opts.IsClientFacing);
        Assert.Equal("Production", opts.ConnectionStringName);
    }

    [Fact]
    public void EnvironmentOptions_DefaultValues()
    {
        var opts = BindOptions<EnvironmentOptions>("Environment", new());

        Assert.Equal("CDT", opts.Role);
        Assert.False(opts.IsClientFacing);
        Assert.Equal("Default", opts.ConnectionStringName);
    }

    [Fact]
    public void GuestOptions_BindsFromSection()
    {
        var opts = BindOptions<GuestOptions>("Guest", new()
        {
            ["Guest:MainGuestCount"] = "2",
            ["Guest:AdditionalGuestCount"] = "3",
            ["Guest:NonParticipatingGuestCount"] = "1"
        });

        Assert.Equal(2, opts.MainGuestCount);
        Assert.Equal(3, opts.AdditionalGuestCount);
        Assert.Equal(1, opts.NonParticipatingGuestCount);
    }

    [Fact]
    public void DisplayOptions_DefaultValues()
    {
        var opts = BindOptions<DisplayOptions>("Display", new());

        Assert.Equal("MM/dd/yyyy", opts.DateFormat);
        Assert.Equal("hh:mm tt", opts.TimeFormat);
        Assert.True(opts.ScrollToWindowTop);
        Assert.False(opts.DisplaySSLSeal);
        Assert.True(opts.DisableForwardScript);
    }

    [Fact]
    public void SmtpOptions_BindsFromSection()
    {
        var opts = BindOptions<SmtpOptions>("Smtp", new()
        {
            ["Smtp:DeliveryMethod"] = "PickupDirectory",
            ["Smtp:PickupDirectoryLocation"] = @"C:\mail",
            ["Smtp:Host"] = "smtp.test.com",
            ["Smtp:Port"] = "587",
            ["Smtp:EnableSsl"] = "true",
            ["Smtp:UserName"] = "user",
            ["Smtp:Password"] = "pass"
        });

        Assert.Equal("PickupDirectory", opts.DeliveryMethod);
        Assert.Equal(@"C:\mail", opts.PickupDirectoryLocation);
        Assert.Equal("smtp.test.com", opts.Host);
        Assert.Equal(587, opts.Port);
        Assert.True(opts.EnableSsl);
        Assert.Equal("user", opts.UserName);
        Assert.Equal("pass", opts.Password);
    }

    [Fact]
    public void ErrorLoggingOptions_DefaultValues()
    {
        var opts = BindOptions<ErrorLoggingOptions>("ErrorLogging", new());

        Assert.Equal(926, opts.SystemId);
        Assert.Equal(4090, opts.MaxMessageChunkSize);
        Assert.Contains("unexpected error", opts.ClientFacingErrorMessage);
    }

    [Fact]
    public void LoginRedirectOptions_DefaultValues()
    {
        var opts = BindOptions<LoginRedirectOptions>("Authentication", new());

        Assert.Equal("Login", opts.MissingAuthenticationTarget);
        Assert.Equal("Login", opts.InvalidNavigationTarget);
    }

    [Fact]
    public void NavigationOptions_BindsLists()
    {
        var opts = BindOptions<NavigationOptions>("Navigation", new()
        {
            ["Navigation:Links:0:Name"] = "Home",
            ["Navigation:Links:0:Url"] = "/home",
            ["Navigation:Links:0:ShowLeft"] = "true",
            ["Navigation:RegistrationFlow:0:Name"] = "profile",
            ["Navigation:RegistrationFlow:0:Level"] = "1",
            ["Navigation:RegistrationFlow:0:Url"] = "/profile"
        });

        Assert.Single(opts.Links);
        Assert.Equal("Home", opts.Links[0].Name);
        Assert.Equal("/home", opts.Links[0].Url);
        Assert.True(opts.Links[0].ShowLeft);
        Assert.Single(opts.RegistrationFlow);
        Assert.Equal("profile", opts.RegistrationFlow[0].Name);
    }

    [Fact]
    public void SamlOptions_BindsFromSection()
    {
        var opts = BindOptions<SamlOptions>("Saml", new()
        {
            ["Saml:Enabled"] = "true",
            ["Saml:IdpSsoUrl"] = "https://idp.example.com/sso",
            ["Saml:SpEntityId"] = "https://sp.example.com",
            ["Saml:DebugMode"] = "true"
        });

        Assert.True(opts.Enabled);
        Assert.Equal("https://idp.example.com/sso", opts.IdpSsoUrl);
        Assert.Equal("https://sp.example.com", opts.SpEntityId);
        Assert.True(opts.DebugMode);
    }

    [Fact]
    public void AppSessionOptions_BindsAllProperties()
    {
        var opts = BindOptions<AppSessionOptions>("Session", new()
        {
            ["Session:TimeoutMinutes"] = "30",
            ["Session:TimeoutWarningSeconds"] = "900",
            ["Session:TimeoutSeconds"] = "120"
        });

        Assert.Equal(30, opts.TimeoutMinutes);
        Assert.Equal(900, opts.TimeoutWarningSeconds);
        Assert.Equal(120, opts.TimeoutSeconds);
    }

    [Fact]
    public void EmailOptions_BindsTemplateProperties()
    {
        var opts = BindOptions<EmailOptions>("Email", new()
        {
            ["Email:SmtpFrom"] = "test@test.com",
            ["Email:RegistrationConfirmationTemplate"] = "RegConfirm.html",
            ["Email:ThirdPartyFromEmail"] = "third@test.com"
        });

        Assert.Equal("test@test.com", opts.SmtpFrom);
        Assert.Equal("RegConfirm.html", opts.RegistrationConfirmationTemplate);
        Assert.Equal("third@test.com", opts.ThirdPartyFromEmail);
    }
}
