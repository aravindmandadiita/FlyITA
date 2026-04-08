using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using FlyITA.Core.Models;
using FlyITA.Infrastructure.Services;

namespace FlyITA.Infrastructure.Tests.Services;

public class LegacyApiCardProcessServiceTests
{
    private readonly Mock<ILogger<LegacyApiCardProcessService>> _loggerMock = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private LegacyApiCardProcessService CreateService(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100/") };
        return new LegacyApiCardProcessService(client, _loggerMock.Object);
    }

    private static PaymentRequest CreateAchRequest() => new()
    {
        BankName = "Test Bank",
        RoutingNumber = "021000021",
        AccountNumber = "123456789",
        AccountHolderName = "John Doe",
        AccountType = "Checking",
        Amount = 150.00m,
        Currency = "USD",
        Description = "ACH Payment",
        ParticipantId = 42
    };

    // ProcessPaymentAsync tests

    [Fact]
    public async Task ProcessPayment_Success_ReturnsPaymentResult()
    {
        var expected = new PaymentResult
        {
            Success = true,
            TransactionId = "TXN-12345",
            ResponseCode = "OK"
        };
        var handler = new MockHttpHandler(HttpStatusCode.OK, JsonSerializer.Serialize(expected, JsonOptions));
        var service = CreateService(handler);

        var result = await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.True(result.Success);
        Assert.Equal("TXN-12345", result.TransactionId);
        Assert.Equal("OK", result.ResponseCode);
    }

    [Fact]
    public async Task ProcessPayment_ServerError_ReturnsFailure()
    {
        var errorResponse = new PaymentResult
        {
            Success = false,
            ErrorMessage = "Internal server error",
            ResponseCode = "ERROR"
        };
        var handler = new MockHttpHandler(HttpStatusCode.InternalServerError, JsonSerializer.Serialize(errorResponse, JsonOptions));
        var service = CreateService(handler);

        var result = await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.False(result.Success);
        Assert.Contains("Internal server error", result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessPayment_Timeout_ReturnsFailure()
    {
        var handler = new TimeoutHttpHandler();
        var service = CreateService(handler);

        var result = await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.False(result.Success);
        Assert.Contains("timed out", result.ErrorMessage);
    }

    [Fact]
    public async Task ProcessPayment_InvalidJson_ReturnsFailure()
    {
        var handler = new MockHttpHandler(HttpStatusCode.OK, "not valid json {{{}}}");
        var service = CreateService(handler);

        var result = await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ProcessPayment_SendsCorrectPayload()
    {
        var expected = new PaymentResult { Success = true, TransactionId = "TXN-1" };
        var handler = new CapturingHttpHandler(HttpStatusCode.OK, JsonSerializer.Serialize(expected, JsonOptions));
        var service = CreateService(handler);

        await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.NotNull(handler.CapturedContent);
        var body = handler.CapturedContent!;
        Assert.Contains("\"bankName\":\"Test Bank\"", body);
        Assert.Contains("\"routingNumber\":\"021000021\"", body);
        Assert.Contains("\"accountNumber\":\"123456789\"", body);
        Assert.Contains("\"accountHolderName\":\"John Doe\"", body);
        Assert.Contains("\"accountType\":\"Checking\"", body);
        Assert.Contains("\"amount\":150", body);
    }

    [Fact]
    public async Task ProcessPayment_SendsToCorrectUrl()
    {
        var expected = new PaymentResult { Success = true, TransactionId = "TXN-1" };
        var handler = new CapturingHttpHandler(HttpStatusCode.OK, JsonSerializer.Serialize(expected, JsonOptions));
        var service = CreateService(handler);

        await service.ProcessPaymentAsync(CreateAchRequest());

        Assert.NotNull(handler.CapturedUri);
        Assert.EndsWith("api/payments/ach", handler.CapturedUri!.ToString());
    }

    // RefundAsync tests

    [Fact]
    public async Task Refund_NotImplemented_ReturnsFailure()
    {
        var handler = new MockHttpHandler(HttpStatusCode.NotImplemented, "{}");
        var service = CreateService(handler);

        var result = await service.RefundAsync("TXN-123", 50.00m);

        Assert.False(result.Success);
        Assert.Contains("not yet available", result.ErrorMessage);
    }

    [Fact]
    public async Task Refund_SendsToCorrectUrl()
    {
        var handler = new CapturingHttpHandler(HttpStatusCode.NotImplemented, "{}");
        var service = CreateService(handler);

        await service.RefundAsync("TXN-123", 50.00m);

        Assert.NotNull(handler.CapturedUri);
        Assert.EndsWith("api/payments/refund", handler.CapturedUri!.ToString());
    }

    // Test helpers

    private class MockHttpHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public MockHttpHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            });
        }
    }

    private class CapturingHttpHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public string? CapturedContent { get; private set; }
        public Uri? CapturedUri { get; private set; }

        public CapturingHttpHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            CapturedUri = request.RequestUri;
            if (request.Content != null)
                CapturedContent = await request.Content.ReadAsStringAsync(ct);

            return new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            };
        }
    }

    private class TimeoutHttpHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            throw new TaskCanceledException("The request timed out.");
        }
    }
}
