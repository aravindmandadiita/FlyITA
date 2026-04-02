using Xunit;
using System.ServiceModel;
using Microsoft.Extensions.Logging;
using Moq;
using FlyITA.Infrastructure.Services;

namespace FlyITA.Infrastructure.Tests.Services;

public class WcfPerformanceCentralClientTests
{
    private readonly Mock<ILogger<WcfPerformanceCentralClient>> _loggerMock = new();

    [Fact]
    public void Constructor_AcceptsValidParameters()
    {
        var service = new WcfPerformanceCentralClient("https://test.example.com/PerformanceCentral.svc", 30, _loggerMock.Object);
        Assert.NotNull(service);
    }

    [Fact]
    public void CreateBinding_ReturnsTransportSecurity()
    {
        var service = new WcfPerformanceCentralClient("https://test.example.com/PerformanceCentral.svc", 30, _loggerMock.Object);
        var binding = service.CreateBinding();
        Assert.Equal(BasicHttpsSecurityMode.Transport, binding.Security.Mode);
        Assert.Equal(TimeSpan.FromSeconds(30), binding.SendTimeout);
    }

    [Fact]
    public async Task GetBookingAsync_ReturnsNull_WhenNotConfigured()
    {
        var service = new WcfPerformanceCentralClient("https://test.example.com/PerformanceCentral.svc", 30, _loggerMock.Object);
        var result = await service.GetBookingAsync(123);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateBookingAsync_ReturnsFalse_WhenNotConfigured()
    {
        var service = new WcfPerformanceCentralClient("https://test.example.com/PerformanceCentral.svc", 30, _loggerMock.Object);
        var result = await service.UpdateBookingAsync(123, new Dictionary<string, object?> { ["key"] = "value" });
        Assert.False(result);
    }
}
