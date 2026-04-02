using Xunit;
using System.ServiceModel;
using Microsoft.Extensions.Logging;
using Moq;
using FlyITA.Core.Models;
using FlyITA.Infrastructure.Services;

namespace FlyITA.Infrastructure.Tests.Services;

public class WcfCardProcessServiceTests
{
    private readonly Mock<ILogger<WcfCardProcessService>> _loggerMock = new();

    [Fact]
    public void Constructor_AcceptsValidParameters()
    {
        var service = new WcfCardProcessService("https://test.example.com/CardProcess.svc", 30, _loggerMock.Object);
        Assert.NotNull(service);
    }

    [Fact]
    public void CreateBinding_ReturnsTransportSecurity()
    {
        var service = new WcfCardProcessService("https://test.example.com/CardProcess.svc", 30, _loggerMock.Object);
        var binding = service.CreateBinding();
        Assert.Equal(BasicHttpsSecurityMode.Transport, binding.Security.Mode);
        Assert.Equal(TimeSpan.FromSeconds(30), binding.SendTimeout);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsFailure_WhenEndpointUrlIsEmpty()
    {
        var service = new WcfCardProcessService("", 30, _loggerMock.Object);
        var request = new PaymentRequest { Token = "test-token", Amount = 100.00m };
        var result = await service.ProcessPaymentAsync(request);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ThrowsNotSupportedException_WhenEndpointUrlIsConfigured()
    {
        var service = new WcfCardProcessService("https://test.example.com/CardProcess.svc", 30, _loggerMock.Object);
        var request = new PaymentRequest { Token = "test-token", Amount = 100.00m };
        await Assert.ThrowsAsync<NotSupportedException>(
            () => service.ProcessPaymentAsync(request));
    }

    [Fact]
    public async Task RefundAsync_ReturnsFailure_WhenEndpointUrlIsEmpty()
    {
        var service = new WcfCardProcessService("", 30, _loggerMock.Object);
        var result = await service.RefundAsync("txn-123", 50.00m);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task RefundAsync_ThrowsNotSupportedException_WhenEndpointUrlIsConfigured()
    {
        var service = new WcfCardProcessService("https://test.example.com/CardProcess.svc", 30, _loggerMock.Object);
        await Assert.ThrowsAsync<NotSupportedException>(
            () => service.RefundAsync("txn-123", 50.00m));
    }
}
