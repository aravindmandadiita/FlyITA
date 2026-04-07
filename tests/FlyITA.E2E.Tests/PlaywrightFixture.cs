using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Xunit;

namespace FlyITA.E2E.Tests;

public class PlaywrightFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public HttpClient HttpClient { get; private set; } = null!;
    public IBrowser Browser => _browser!;
    public string BaseUrl { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        // WebApplicationFactory starts a TestServer. Playwright needs a real TCP listener.
        // For now, use the TestServer base address — E2E tests that use Playwright browser
        // require `pwsh playwright.ps1 install` AND a running server.
        // HttpClient-based tests (SmokeTests.Health/ImageListing) work without Playwright.
        _factory = new WebApplicationFactory<Program>();
        HttpClient = _factory.CreateClient();
        BaseUrl = _factory.Server.BaseAddress.ToString().TrimEnd('/');

        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser != null) await _browser.DisposeAsync();
        _playwright?.Dispose();
        HttpClient?.Dispose();
        if (_factory != null) await _factory.DisposeAsync();
    }
}
