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
        var response = await page.GotoAsync(_fixture.BaseUrl);

        Assert.NotNull(response);
        Assert.True(response!.Ok);

        var title = await page.TitleAsync();
        Assert.Contains("FlyITA", title);

        await page.CloseAsync();
    }
}
