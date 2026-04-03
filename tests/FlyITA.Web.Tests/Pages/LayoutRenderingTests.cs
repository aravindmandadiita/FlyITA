using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests.Pages;

public class LayoutRenderingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LayoutRenderingTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task IndexPage_RendersWithLayout()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Layout structure
        Assert.Contains("<html lang=\"en-us\"", content);
        Assert.Contains("<header class=\"header\"", content);
        Assert.Contains("<main id=\"begin-content\"", content);
        Assert.Contains("<footer class=\"main-footer\"", content);
        Assert.Contains("id=\"footer-copyright\"", content);

        // Accessibility
        Assert.Contains("Skip to main content", content);
        Assert.Contains("role=\"banner\"", content);
        Assert.Contains("role=\"main\"", content);
        Assert.Contains("role=\"contentinfo\"", content);

        // Vendor libs (local, not CDN)
        Assert.Contains("bootstrap/css/bootstrap.min.css", content);
        Assert.Contains("font-awesome/css/all.min.css", content);

        // Site assets
        Assert.Contains("css/theme.css", content);
        Assert.Contains("css/main.css", content);
        Assert.Contains("js/site.js", content);
        Assert.Contains("js/navigation.js", content);
        Assert.Contains("js/theme.js", content);
    }

    [Fact]
    public async Task IndexPage_ShowsHomeBanner()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("id=\"banner-home\"", content);
        Assert.DoesNotContain("id=\"banner-site\"", content);
    }

    [Fact]
    public async Task IndexPage_NoJQueryReferences()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("jquery.min.js", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("jquery-ui", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("modernizr", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("moment.min.js", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IndexPage_NoCdnReferences()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.DoesNotContain("cdn.jsdelivr.net", content);
        Assert.DoesNotContain("cdnjs.cloudflare.com", content);
        Assert.DoesNotContain("kit.fontawesome.com", content);
    }

    [Fact]
    public async Task IndexPage_ContainsItineraryModal()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("id=\"modalItinerary\"", content);
        Assert.Contains("id=\"ReservationNumber\"", content);
        Assert.Contains("id=\"TravelerLastName\"", content);
        Assert.Contains("data-bs-dismiss=\"modal\"", content);
    }

    [Fact]
    public async Task IndexPage_ContainsMainMenu()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("aria-label=\"Main navigation\"", content);
        Assert.Contains("Travel Reservations", content);
        Assert.Contains("Resources", content);
    }

    [Fact]
    public async Task ErrorPage_RendersWithLayout()
    {
        var response = await _client.GetAsync("/Error");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("<header class=\"header\"", content);
        Assert.Contains("<main id=\"begin-content\"", content);
        Assert.Contains("<footer", content);
    }
}
