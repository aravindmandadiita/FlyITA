using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class ColorContrastTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public ColorContrastTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData("/", "Home")]
    [InlineData("/about", "About")]
    [InlineData("/contact", "Contact")]
    [InlineData("/guest-profile", "GuestProfile")]
    [InlineData("/traveler-profile", "TravelerProfile")]
    [InlineData("/vacation", "Vacation")]
    [InlineData("/ach-payment", "AchPayment")]
    public async Task Page_MeetsWCAG_AA_ColorContrast(string path, string pageName)
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}{path}");

            var results = await page.RunAxe();

            var contrastViolations = results.Violations
                .Where(v => v.Id == "color-contrast")
                .ToList();

            Assert.True(
                contrastViolations.Count == 0,
                $"{pageName}: {contrastViolations.Sum(v => v.Nodes.Count())} elements fail WCAG AA color contrast. " +
                string.Join("; ", contrastViolations.SelectMany(v => v.Nodes)
                    .Select(n => n.Html?.Substring(0, Math.Min(n.Html?.Length ?? 0, 80)))));
        }
        finally { await page.CloseAsync(); }
    }
}
