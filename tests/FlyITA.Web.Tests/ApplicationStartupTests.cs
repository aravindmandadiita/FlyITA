using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests;

public class ApplicationStartupTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApplicationStartupTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Application_Starts_Without_Error()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthCheck_Returns_Healthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LandingPage_Returns_200()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Travel with Confidence", content);
    }
}
