using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class SystemPageTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public SystemPageTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ErrorPage_404_ShowsNotFoundMessage()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/error?code=404");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Page Not Found");
        await Assertions.Expect(page.Locator("text=does not exist")).ToBeVisibleAsync();

        await page.CloseAsync();
    }

    [Fact]
    public async Task ErrorPage_Default_ShowsGenericError()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/error");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Error");

        await page.CloseAsync();
    }

    [Fact]
    public async Task ThankYou_Default_ShowsRegistrationSuccess()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/thank-you");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Thank You");
        await Assertions.Expect(page.Locator("text=registration has been completed")).ToBeVisibleAsync();

        await page.CloseAsync();
    }

    [Fact]
    public async Task ThankYou_Vacation_ShowsVacationSuccess()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/thank-you?page=Vacation");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Vacation Travel Request");

        await page.CloseAsync();
    }

    [Theory]
    [InlineData("", "logged out successfully")]
    [InlineData("?msg=timeout", "session has expired")]
    [InlineData("?msg=unknownpax", "not valid for this program")]
    [InlineData("?msg=unknownprog", "program was not found")]
    [InlineData("?msg=locked", "locked")]
    public async Task Logout_ShowsCorrectMessage(string queryString, string expectedText)
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/logout{queryString}");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Logged Out");
        await Assertions.Expect(page.Locator($"text={expectedText}")).ToBeVisibleAsync();

        await page.CloseAsync();
    }

    [Fact]
    public async Task AccessDenied_ShowsDeniedMessage()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/access-denied");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Access Denied");
        await Assertions.Expect(page.Locator("text=do not have permission")).ToBeVisibleAsync();

        await page.CloseAsync();
    }

    [Fact]
    public async Task Closed_ShowsRegistrationClosedMessage()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync($"{_fixture.BaseUrl}/closed");

        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Registration Closed");

        await page.CloseAsync();
    }
}
