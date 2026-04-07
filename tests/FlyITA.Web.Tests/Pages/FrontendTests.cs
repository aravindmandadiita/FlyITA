using System.Net;
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
    public async Task Layout_ContainsViewportMetaTag()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("name=\"viewport\"", content);
        Assert.Contains("width=device-width", content);
    }

    [Fact]
    public async Task Layout_ContainsDescriptionMetaTag()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("name=\"description\"", content);
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

        Assert.Contains("navigation.js", content);
        Assert.Contains("theme.js", content);
        Assert.Contains("site.js", content);
        // All custom JS should have defer
        Assert.Matches("navigation\\.js.*?defer", content);
        Assert.Matches("theme\\.js.*?defer", content);
        Assert.Matches("site\\.js.*?defer", content);
    }

    // Accessibility elements

    [Fact]
    public async Task Layout_ContainsSkipToContentLink()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("Skip to main content", content);
        Assert.Contains("visually-hidden-focusable", content);
    }

    [Fact]
    public async Task Layout_ContainsAriaLandmarks()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("role=\"banner\"", content);
        Assert.Contains("role=\"main\"", content);
        Assert.Contains("role=\"contentinfo\"", content);
    }

    [Fact]
    public async Task Layout_FooterLogoHasAltText()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("alt=\"ITA Group logo\"", content);
    }

    [Fact]
    public async Task Layout_ScrollTopButtonHasAriaLabel()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("aria-label=\"Scroll to top\"", content);
    }

    // Bootstrap 5 loaded

    [Fact]
    public async Task Layout_LoadsBootstrap5()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.Contains("bootstrap.min.css", content);
        Assert.Contains("bootstrap.bundle.min.js", content);
    }

    // No jQuery

    [Fact]
    public async Task Layout_DoesNotLoadJQuery()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/")).Content.ReadAsStringAsync();

        Assert.DoesNotContain("jquery.min.js", content);
        Assert.DoesNotContain("jquery.js", content);
    }
}
