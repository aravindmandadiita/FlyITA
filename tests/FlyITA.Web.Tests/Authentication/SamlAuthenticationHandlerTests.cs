using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Authentication;
using FlyITA.Web.Options;

namespace FlyITA.Web.Tests.Authentication;

public class SamlAuthenticationHandlerTests
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly Mock<ILoggerFactory> _loggerFactoryMock = new();
    private readonly Mock<ILogger<SamlAuthenticationHandler>> _loggerMock = new();

    public SamlAuthenticationHandlerTests()
    {
        _loggerFactoryMock.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
    }

    [Fact]
    public async Task HandleAuthenticate_ReturnsNoResult_WhenDisabled()
    {
        var options = new SamlOptions { Enabled = false };
        var handler = await CreateHandlerAsync(options);

        var result = await handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.Null(result.Failure);
        Assert.Null(result.Ticket);
    }

    [Fact]
    public async Task HandleAuthenticate_ReturnsSuccess_WhenSessionExists()
    {
        var options = new SamlOptions { Enabled = true };
        _contextMock.SetupGet(c => c.SAMLSessionID).Returns("session-123");
        _contextMock.SetupGet(c => c.RealUserID).Returns(42);

        var handler = await CreateHandlerAsync(options);
        var result = await handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.Equal("42", result.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [Fact]
    public async Task HandleAuthenticate_ReturnsNoResult_WhenNoSessionAndNoResponse()
    {
        var options = new SamlOptions { Enabled = true };
        _contextMock.SetupGet(c => c.SAMLSessionID).Returns(string.Empty);

        var handler = await CreateHandlerAsync(options);
        var result = await handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.Null(result.Failure);
    }

    [Fact]
    public async Task HandleChallenge_RedirectsToIdp_WhenEnabled()
    {
        var options = new SamlOptions
        {
            Enabled = true,
            IdpSsoUrl = "https://idp.example.com/sso",
            SpEntityId = "https://sp.example.com",
            AssertionConsumerServiceUrl = "https://sp.example.com/acs"
        };

        var context = new DefaultHttpContext();
        var handler = await CreateHandlerAsync(options, context);

        await handler.ChallengeAsync(new AuthenticationProperties());

        Assert.Equal(302, context.Response.StatusCode);
        Assert.StartsWith("https://idp.example.com/sso?SAMLRequest=", context.Response.Headers.Location.ToString());
    }

    [Fact]
    public async Task HandleChallenge_DoesNotRedirect_WhenDisabled()
    {
        var options = new SamlOptions { Enabled = false };
        var context = new DefaultHttpContext();
        var handler = await CreateHandlerAsync(options, context);

        await handler.ChallengeAsync(new AuthenticationProperties());

        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task HandleAuthenticate_RejectsSamlResponse_WhenNoCertAndNotDebug()
    {
        var options = new SamlOptions
        {
            Enabled = true,
            DebugMode = false,
            IdpCertificateFile = "" // No certificate
        };
        _contextMock.SetupGet(c => c.SAMLSessionID).Returns(string.Empty);

        // Build a minimal SAML response (Base64-encoded)
        var samlXml = @"<samlp:Response xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
            <saml:Assertion xmlns:saml=""urn:oasis:names:tc:SAML:2.0:assertion"">
                <saml:NameID>user@test.com</saml:NameID>
            </saml:Assertion>
        </samlp:Response>";
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(samlXml));

        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.ContentType = "application/x-www-form-urlencoded";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes($"SAMLResponse={Uri.EscapeDataString(base64)}"));

        var handler = await CreateHandlerAsync(options, context);
        var result = await handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Contains("certificate", result.Failure!.Message);
    }

    [Fact]
    public async Task HandleSignOut_RedirectsToIdpSlo()
    {
        var options = new SamlOptions
        {
            Enabled = true,
            IdpSloUrl = "https://idp.example.com/slo",
            SpEntityId = "https://sp.example.com"
        };
        _contextMock.SetupProperty(c => c.SAMLSessionID);
        _contextMock.SetupProperty(c => c.LoginType);

        var context = new DefaultHttpContext();
        var handler = await CreateHandlerAsync(options, context);

        await handler.SignOutAsync(new AuthenticationProperties());

        Assert.StartsWith("https://idp.example.com/slo?SAMLRequest=", context.Response.Headers.Location.ToString());
    }

    private async Task<SamlAuthenticationHandler> CreateHandlerAsync(SamlOptions options, HttpContext? httpContext = null)
    {
        httpContext ??= new DefaultHttpContext();

        var optionsMonitor = new Mock<IOptionsMonitor<SamlOptions>>();
        optionsMonitor.SetupGet(o => o.CurrentValue).Returns(options);
        optionsMonitor.Setup(o => o.Get(It.IsAny<string>())).Returns(options);

        var handler = new SamlAuthenticationHandler(
            optionsMonitor.Object,
            _loggerFactoryMock.Object,
            System.Text.Encodings.Web.UrlEncoder.Default,
            _contextMock.Object);

        var scheme = new AuthenticationScheme("Saml", "Saml", typeof(SamlAuthenticationHandler));
        await handler.InitializeAsync(scheme, httpContext);

        return handler;
    }
}
