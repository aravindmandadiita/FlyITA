using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class FormTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public FormTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GuestProfile_HasRequiredFormFields()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/guest-profile");
            await Assertions.Expect(page.Locator("input[name='FirstName']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("input[name='LastName']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("input[name='Email']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("button[type='submit']")).ToBeVisibleAsync();
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task TravelerProfile_HasRepeaterSections()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/traveler-profile");
            await Assertions.Expect(page.Locator("text=Frequent Flyer Programs")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("text=Hotel Memberships")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("#add-frequent-flyer")).ToBeVisibleAsync();
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task Vacation_SubmitRedirectsToThankYou()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/vacation");
            await page.FillAsync("input[name='NameOfPersonRequesting']", "John Doe");
            await page.FillAsync("input[name='GeneralAndPassengerEmail']", "john@example.com");
            await page.FillAsync("input[name='Passengers[0].FirstName']", "John");
            await page.FillAsync("input[name='Passengers[0].LastName']", "Doe");
            await page.FillAsync("input[name='Passengers[0].DateOfBirth']", "1990-01-15");
            await page.SelectOptionAsync("select[name='Passengers[0].Gender']", "Male");
            await page.FillAsync("input[name='DepartureCity']", "Chicago");
            await page.FillAsync("textarea[name='DestinationsInterestedIn']", "Hawaii");
            await page.ClickAsync("button[type='submit']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Assert.Contains("/thank-you", page.Url);
        }
        finally { await page.CloseAsync(); }
    }

    [Fact]
    public async Task AchPayment_HasBankFormFields()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/ach-payment");
            await Assertions.Expect(page.Locator("input[name='BankName']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("input[name='RoutingNumber']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("input[name='AccountNumber']")).ToBeVisibleAsync();
            await Assertions.Expect(page.Locator("input[name='Amount']")).ToBeVisibleAsync();
        }
        finally { await page.CloseAsync(); }
    }
}
