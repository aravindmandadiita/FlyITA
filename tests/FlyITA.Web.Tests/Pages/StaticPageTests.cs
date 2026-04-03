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
}
