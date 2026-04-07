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
    public async Task VacationRequest_SubmitShowsSuccessMessage()
    {
        var page = await _fixture.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/vacation-request");
            await page.FillAsync("input[name='TravelerName']", "John Doe");
            await page.FillAsync("input[name='TravelerEmail']", "john@example.com");
            await page.FillAsync("input[name='Destination']", "Paris");
            await page.FillAsync("input[name='DepartureDate']", "2026-06-01");
            await page.FillAsync("input[name='ReturnDate']", "2026-06-15");
            await page.ClickAsync("button[type='submit']");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Assertions.Expect(page.Locator(".alert-success")).ToBeVisibleAsync();
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
