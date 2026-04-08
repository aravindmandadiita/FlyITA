using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class AxeAccessibilityTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public AxeAccessibilityTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("/", "Home")]
    [InlineData("/about", "About")]
    [InlineData("/contact", "Contact")]
    [InlineData("/privacy", "Privacy")]
    [InlineData("/international", "International")]
    [InlineData("/programs", "Programs")]
    [InlineData("/reservations", "Reservations")]
    [InlineData("/resources", "Resources")]
    [InlineData("/travel-info", "TravelInfo")]
    [InlineData("/guest-profile", "GuestProfile")]
    [InlineData("/traveler-profile", "TravelerProfile")]
    [InlineData("/vacation", "Vacation")]
    [InlineData("/ach-payment", "AchPayment")]
    [InlineData("/error", "Error")]
    [InlineData("/thank-you", "ThankYou")]
    [InlineData("/logout", "Logout")]
    [InlineData("/access-denied", "AccessDenied")]
    [InlineData("/closed", "Closed")]
    public async Task Page_HasNoCriticalOrSeriousA11yViolations(string path, string pageName)
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}{path}");

            var results = await page.RunAxe();

            var critical = results.Violations.Where(v => v.Impact == "critical").ToList();
            var serious = results.Violations.Where(v => v.Impact == "serious").ToList();

            Assert.True(
                critical.Count == 0,
                $"{pageName}: {critical.Count} critical a11y violations: " +
                string.Join(", ", critical.Select(v => $"{v.Id} ({v.Nodes.Count()} nodes)")));

            Assert.True(
                serious.Count == 0,
                $"{pageName}: {serious.Count} serious a11y violations: " +
                string.Join(", ", serious.Select(v => $"{v.Id} ({v.Nodes.Count()} nodes)")));
        }
        finally { await page.CloseAsync(); }
    }
}
