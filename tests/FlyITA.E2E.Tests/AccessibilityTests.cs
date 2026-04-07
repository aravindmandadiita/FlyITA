using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class AccessibilityTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public AccessibilityTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_HasProperAriaLandmarks()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            await Assertions.Expect(page.Locator("header[role='banner']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("main[role='main']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("footer[role='contentinfo']")).ToBeVisibleAsync();
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_HasSingleH1()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var h1Count = await page.Locator("h1").CountAsync();
            Assert.Equal(1, h1Count);
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_ImagesHaveAltText()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var images = page.Locator("img");
            var count = await images.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var alt = await images.Nth(i).GetAttributeAsync("alt");
                Assert.False(string.IsNullOrEmpty(alt), $"Image at index {i} is missing alt text");
            }
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_NoJQueryLoaded()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var hasJQuery = await page.EvaluateAsync<bool>("() => typeof jQuery !== 'undefined' || typeof $ === 'function'");
            Assert.False(hasJQuery, "jQuery should not be loaded on the page");
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_BootstrapModalAccessible()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var modal = page.Locator(".modal");
            var count = await modal.CountAsync();
            if (count > 0)
            {
                var role = await modal.First.GetAttributeAsync("role");
                Assert.True(role == "dialog" || role == null, "Modal should have dialog role or rely on BS5 default");
            }
        }
        finally { await page.CloseAsync(); }
    }
}
