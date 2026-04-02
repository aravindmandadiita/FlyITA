using Xunit;
using System.ServiceModel;
using Microsoft.Extensions.Logging;
using Moq;
using FlyITA.Infrastructure.Services;

namespace FlyITA.Infrastructure.Tests.Services;

public class WcfCardTokenServiceTests
{
    private readonly Mock<ILogger<WcfCardTokenService>> _loggerMock = new();

    [Fact]
    public void Constructor_AcceptsValidParameters()
    {
        var service = new WcfCardTokenService("https://test.example.com/CardToken.svc", 30, _loggerMock.Object);
        Assert.NotNull(service);
    }

    [Fact]
    public void CreateBinding_ReturnsTransportSecurity()
    {
        var service = new WcfCardTokenService("https://test.example.com/CardToken.svc", 30, _loggerMock.Object);
        var binding = service.CreateBinding();
        Assert.Equal(BasicHttpsSecurityMode.Transport, binding.Security.Mode);
        Assert.Equal(TimeSpan.FromSeconds(30), binding.SendTimeout);
    }

    [Fact]
    public async Task TokenizeCardAsync_ReturnsNull_WhenNotConfigured()
    {
        var service = new WcfCardTokenService("https://test.example.com/CardToken.svc", 30, _loggerMock.Object);
        var result = await service.TokenizeCardAsync("4111111111111111", "12/25");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCardInfoByTokenAsync_ReturnsNull_WhenNotConfigured()
    {
        var service = new WcfCardTokenService("https://test.example.com/CardToken.svc", 30, _loggerMock.Object);
        var result = await service.GetCardInfoByTokenAsync("test-token");
        Assert.Null(result);
    }
}
