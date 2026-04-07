using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests.Pages;

public class SystemPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SystemPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Page rendering tests

    [Theory]
    [InlineData("/thank-you", "Thank You")]
    [InlineData("/closed", "Registration Closed")]
    [InlineData("/error", "Error")]
    [InlineData("/logout", "Logged Out")]
    [InlineData("/access-denied", "Access Denied")]
    public async Task SystemPage_Returns200_WithTitle(string url, string expectedTitle)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(expectedTitle, content);
    }

    // ThankYou query string handling

    [Fact]
    public async Task ThankYou_Default_ShowsRegistrationMessage()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/thank-you")).Content.ReadAsStringAsync();

        Assert.Contains("Your registration has been completed successfully", content);
    }

    [Fact]
    public async Task ThankYou_VacationPage_ShowsVacationMessage()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/thank-you?page=Vacation")).Content.ReadAsStringAsync();

        Assert.Contains("Vacation Travel Request", content);
        Assert.Contains("vacation travel request has been submitted", content);
    }

    // Error page differentiation

    [Fact]
    public async Task Error_404Code_ShowsNotFoundMessageAndStatus()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/error?code=404");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Page Not Found", content);
        Assert.Contains("does not exist", content);
    }

    [Fact]
    public async Task Error_NoCode_Returns200WithGenericMessage()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/error");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Error", content);
    }

    // Logout message handling

    [Theory]
    [InlineData("/logout", "logged out successfully")]
    [InlineData("/logout?msg=unknownpax", "not valid for this program")]
    [InlineData("/logout?msg=unknownprog", "program was not found")]
    [InlineData("/logout?msg=timeout", "session has expired")]
    public async Task Logout_ShowsCorrectMessage(string url, string expectedContent)
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync(url)).Content.ReadAsStringAsync();

        Assert.Contains(expectedContent, content);
    }

    [Fact]
    public async Task Logout_Locked_ShowsLockedAccountMessage()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/logout?msg=locked")).Content.ReadAsStringAsync();

        Assert.Contains("locked", content);
        Assert.Contains("contact", content);
    }

    // AccessDenied content

    [Fact]
    public async Task AccessDenied_ShowsDeniedMessage()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/access-denied")).Content.ReadAsStringAsync();

        Assert.Contains("do not have permission", content);
    }

    // ImageListing API

    [Fact]
    public async Task ImageListing_ReturnsEmptyJsonArray()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/images");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.Equal(0, json.RootElement.GetArrayLength());
    }

    // Legacy URL redirect tests (preserve query string)

    [Theory]
    [InlineData("/ThankYou.aspx", "/thank-you")]
    [InlineData("/closed.aspx", "/closed")]
    [InlineData("/error.aspx", "/error")]
    [InlineData("/logout.aspx", "/logout")]
    [InlineData("/Accessdenied.aspx", "/access-denied")]
    [InlineData("/ImageListing.aspx", "/api/images")]
    public async Task LegacySystemUrl_Returns301_ToModernPath(string legacyUrl, string expectedPath)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(legacyUrl);

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.StartsWith(expectedPath, response.Headers.Location?.OriginalString ?? "");
    }

    [Fact]
    public async Task LegacyLogout_PreservesQueryString()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/logout.aspx?msg=timeout");

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal("/logout?msg=timeout", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task LegacyThankYou_PreservesQueryString()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/ThankYou.aspx?page=Vacation");

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal("/thank-you?page=Vacation", response.Headers.Location?.OriginalString);
    }
}
