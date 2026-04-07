using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class SmokeTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public SmokeTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_Loads_Successfully()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            var response = await page.GotoAsync(_fixture.BaseUrl);

            Assert.NotNull(response);
            Assert.True(response!.Ok);

            var title = await page.TitleAsync();
            Assert.Contains("Fly ITA", title);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Theory]
    [InlineData("/", "Fly ITA")]
    [InlineData("/about", "About ITA")]
    [InlineData("/contact", "Contact Us")]
    [InlineData("/privacy", "Privacy")]
    [InlineData("/international", "International Travel")]
    [InlineData("/programs", "Airline Programs")]
    [InlineData("/reservations", "Booking Information")]
    [InlineData("/resources", "Travel Updates")]
    [InlineData("/travel-info", "Travel Best Practices")]
    [InlineData("/guest-profile", "Guest Profile")]
    [InlineData("/traveler-profile", "Traveler Profile")]
    [InlineData("/vacation", "Vacation Travel Request")]
    [InlineData("/ach-payment", "ACH Payment")]
    [InlineData("/error", "Error")]
    [InlineData("/thank-you", "Thank You")]
    [InlineData("/logout", "Logged Out")]
    [InlineData("/access-denied", "Access Denied")]
    [InlineData("/closed", "Registration Closed")]
    public async Task Page_LoadsWithCorrectTitle(string path, string expectedTitleFragment)
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            var response = await page.GotoAsync($"{_fixture.BaseUrl}{path}");

            Assert.NotNull(response);
            Assert.True(response!.Ok);

            var title = await page.TitleAsync();
            Assert.Contains(expectedTitleFragment, title);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task HealthEndpoint_Returns200()
    {
        var response = await _fixture.HttpClient.GetAsync("/health");
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task ImageListingApi_ReturnsEmptyArray()
    {
        var response = await _fixture.HttpClient.GetAsync("/api/images");
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("[]", content.Trim());
    }
}
