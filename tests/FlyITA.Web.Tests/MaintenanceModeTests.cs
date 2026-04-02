using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlyITA.Web.Tests;

public class MaintenanceModeTests
{
    [Fact]
    public async Task Returns_503_When_DownHtml_Exists()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"flyita-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);
        var downPath = Path.Combine(tempDir, "down.html");

        try
        {
            await File.WriteAllTextAsync(downPath, "<h1>Maintenance</h1>");

            await using var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot(tempDir);
                });

            var client = factory.CreateClient();
            var response = await client.GetAsync("/");
            Assert.Equal(System.Net.HttpStatusCode.ServiceUnavailable, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Maintenance", content);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task Returns_Normal_Response_When_No_DownHtml()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
