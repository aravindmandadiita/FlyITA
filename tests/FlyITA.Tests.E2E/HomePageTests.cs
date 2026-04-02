using Microsoft.Playwright;

namespace FlyITA.Tests.E2E;

/// <summary>
/// End-to-end tests using Playwright.
/// These tests require the application to be running and Playwright browsers installed.
/// Run `pwsh tests/FlyITA.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install` before executing.
/// </summary>
public class HomePageTests
{
    [Fact(Skip = "Requires running application and Playwright browsers installed.")]
    public async Task HomePage_IsReachable()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        var response = await page.GotoAsync("http://localhost:5000/health");
        Assert.NotNull(response);
        Assert.True(response.Ok);
    }
}
