using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests;

public class SecurityHeadersTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Response_Contains_CSP_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        var value = response.Headers.GetValues("Content-Security-Policy").First();
        Assert.Contains("default-src https:", value);
        Assert.Contains("frame-ancestors 'self'", value);
    }

    [Fact]
    public async Task Response_Contains_XFrameOptions_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.Equal("SAMEORIGIN", response.Headers.GetValues("X-Frame-Options").First());
    }

    [Fact]
    public async Task Response_Contains_XContentTypeOptions_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
    }

    [Fact]
    public async Task Response_Contains_XXSSProtection_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("X-XSS-Protection"));
        Assert.Equal("1; mode=block", response.Headers.GetValues("X-XSS-Protection").First());
    }

    [Fact]
    public async Task Response_Contains_ReferrerPolicy_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("Referrer-Policy"));
        Assert.Equal("no-referrer-when-downgrade", response.Headers.GetValues("Referrer-Policy").First());
    }

    [Fact]
    public async Task Response_Contains_PermissionsPolicy_Header()
    {
        var response = await _client.GetAsync("/");
        Assert.True(response.Headers.Contains("Permissions-Policy"));
        Assert.Equal("microphone=(),camera=(self),geolocation=(self)", response.Headers.GetValues("Permissions-Policy").First());
    }

    [Fact]
    public async Task Response_Contains_CacheControl_Header()
    {
        var response = await _client.GetAsync("/");
        var cacheControl = response.Headers.CacheControl;
        Assert.NotNull(cacheControl);
        Assert.True(cacheControl!.Private);
        Assert.Equal(TimeSpan.Zero, cacheControl.MaxAge);
    }
}
