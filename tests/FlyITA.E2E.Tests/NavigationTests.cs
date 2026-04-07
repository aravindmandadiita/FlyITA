using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class NavigationTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public NavigationTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_HasMainNavigation()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            await Assertions.Expect(page.Locator("header[role='banner']")).ToBeVisibleAsync();
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_HasFooterWithLinks()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var footer = page.Locator("footer[role='contentinfo']");
            await Assertions.Expect(footer).ToBeVisibleAsync();
            var count = await footer.Locator("a").CountAsync();
            Assert.True(count > 5, "Footer should have multiple navigation links");
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task Footer_AboutLink_NavigatesToAboutPage()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            await page.Locator("footer[role='contentinfo'] a[href='/about']").ClickAsync();
            await page.WaitForURLAsync("**/about");
            var title = await page.TitleAsync();
            Assert.Contains("About ITA", title);
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task HomePage_HasSkipToContentLink()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(_fixture.BaseUrl);
            var skipLink = page.Locator("a.skip-nav");
            await Assertions.Expect(skipLink).ToHaveAttributeAsync("href", "#begin-content");
        }
        finally { await page.CloseAsync(); }
    }

    [Theory]
    [InlineData("/default", "/")]
    [InlineData("/home.aspx", "/")]
    [InlineData("/aboutita.aspx", "/about")]
    [InlineData("/privacy.aspx", "/privacy")]
    public async Task LegacyUrl_RedirectsToModernPath(string legacyPath, string expectedPath)
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}{legacyPath}");
            Assert.EndsWith(expectedPath, new Uri(page.Url).AbsolutePath);
        }
        finally { await page.CloseAsync(); }
    }
}
