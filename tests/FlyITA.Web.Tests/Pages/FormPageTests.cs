using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests.Pages;

public class FormPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FormPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Page rendering tests

    [Theory]
    [InlineData("/guest-profile", "Guest Profile Information")]
    [InlineData("/traveler-profile", "Traveler Profile Information")]
    [InlineData("/vacation", "Vacation Travel Request")]
    [InlineData("/ach-payment", "ACH Payment")]
    public async Task FormPage_Returns200_WithTitle(string url, string expectedTitle)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(expectedTitle, content);
    }

    // Legacy URL redirect tests

    [Theory]
    [InlineData("/GuestProfileInformation.aspx", "/guest-profile")]
    [InlineData("/Travelerprofileinformation.aspx", "/traveler-profile")]
    [InlineData("/VacationTravelRequest.aspx", "/vacation")]
    [InlineData("/vacation.aspx", "/vacation")]
    [InlineData("/AchPayment.aspx", "/ach-payment")]
    [InlineData("/vacation-request", "/vacation")]
    public async Task LegacyFormUrl_Returns301_ToModernPath(string legacyUrl, string expectedPath)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(legacyUrl);

        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal(expectedPath, response.Headers.Location?.OriginalString);
    }

    // Form structure tests

    [Fact]
    public async Task GuestProfile_ContainsRequiredFormFields()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/guest-profile")).Content.ReadAsStringAsync();

        Assert.Contains("FirstName", content);
        Assert.Contains("LastName", content);
        Assert.Contains("Email", content);
        Assert.Contains("Save Profile", content);
    }

    [Fact]
    public async Task TravelerProfile_ContainsRequiredFormFields()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/traveler-profile")).Content.ReadAsStringAsync();

        Assert.Contains("FirstName", content);
        Assert.Contains("LastName", content);
        Assert.Contains("Frequent Flyer Programs", content);
        Assert.Contains("Hotel Memberships", content);
        Assert.Contains("Rental Car Preferences", content);
    }

    [Fact]
    public async Task Vacation_ContainsRequiredFormFields()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/vacation")).Content.ReadAsStringAsync();

        Assert.Contains("NameOfPersonRequesting", content);
        Assert.Contains("GeneralAndPassengerEmail", content);
        Assert.Contains("Passengers", content);
        Assert.Contains("DepartureCity", content);
        Assert.Contains("DestinationsInterestedIn", content);
        Assert.Contains("Submit Vacation Request", content);
    }

    [Fact]
    public async Task AchPayment_ContainsRequiredFormFields()
    {
        var client = _factory.CreateClient();

        var content = await (await client.GetAsync("/ach-payment")).Content.ReadAsStringAsync();

        Assert.Contains("BankName", content);
        Assert.Contains("RoutingNumber", content);
        Assert.Contains("AccountNumber", content);
        Assert.Contains("Submit Payment", content);
    }
}
