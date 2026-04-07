using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests.Pages;

public class FrontendTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FrontendTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Meta tags

    [Fact]
    public async Task Layout_ContainsDescriptionMetaTag()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("name=\"description\"", content);
        Assert.Contains("International Travel Associates", content);
    }

    [Fact]
    public async Task Layout_ContainsRobotsMetaTag()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("name=\"robots\"", content);
    }

    // GA4 — disabled by default in test environment

    [Fact]
    public async Task Layout_DoesNotContainGA4_WhenDisabled()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.DoesNotContain("googletagmanager.com", content);
        Assert.DoesNotContain("gtag(", content);
    }

    // JS defer attributes

    [Fact]
    public async Task Layout_JsScriptsHaveDeferAttribute()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Matches("navigation\\.js.*?defer", content);
        Assert.Matches("theme\\.js.*?defer", content);
        Assert.Matches("site\\.js.*?defer", content);
    }
}
