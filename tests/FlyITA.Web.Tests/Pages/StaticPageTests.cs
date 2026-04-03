using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests.Pages;

public class StaticPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StaticPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/default", "/")]
    [InlineData("/default.aspx", "/")]
    [InlineData("/home", "/")]
    [InlineData("/home.aspx", "/")]
    [InlineData("/aboutita.aspx", "/about")]
    [InlineData("/international.aspx", "/international")]
    [InlineData("/privacy.aspx", "/privacy")]
    [InlineData("/helpsite.aspx", "/contact")]
    [InlineData("/resources.aspx", "/resources")]
    [InlineData("/travelinfo.aspx", "/travel-info")]
    [InlineData("/programs.aspx", "/programs")]
    [InlineData("/reservations.aspx", "/reservations")]
    public async Task LegacyUrl_Returns301_ToModernPath(string legacyUrl, string expectedPath)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(legacyUrl);

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal(expectedPath, response.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData("/about", "About ITA")]
    [InlineData("/international", "International Travel")]
    [InlineData("/privacy", "Privacy and Security Policies")]
    [InlineData("/contact", "Contact Us")]
    [InlineData("/resources", "Travel Updates")]
    [InlineData("/travel-info", "Travel Best Practices")]
    [InlineData("/programs", "Airline Programs")]
    [InlineData("/reservations", "Booking Information")]
    public async Task StaticPage_Returns200_WithTitle(string url, string expectedTitle)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(expectedTitle, content);
        Assert.Contains("<header class=\"header\"", content);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/about")]
    [InlineData("/international")]
    [InlineData("/privacy")]
    [InlineData("/contact")]
    [InlineData("/resources")]
    [InlineData("/travel-info")]
    [InlineData("/programs")]
    [InlineData("/reservations")]
    public async Task Page_ContainsNoInternalAspxLinks(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        // Internal .aspx links: href="/something.aspx" (relative path starting with /)
        // External links like href="https://www.united.com/...aspx" are OK
        Assert.False(
            System.Text.RegularExpressions.Regex.IsMatch(content, @"href=""/[^""]*\.aspx"),
            $"Page {url} contains internal .aspx link references");
    }
}
