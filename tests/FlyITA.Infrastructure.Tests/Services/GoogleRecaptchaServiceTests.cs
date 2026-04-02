using Xunit;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using FlyITA.Core.Options;
using FlyITA.Infrastructure.Services;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace FlyITA.Infrastructure.Tests.Services;

public class GoogleRecaptchaServiceTests
{
    private static RecaptchaOptions DefaultOptions(bool enabled = true) => new()
    {
        Enabled = enabled,
        SecretKey = "test-secret",
        MinimumScore = 0.5
    };

    private static HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string content)
    {
        var handler = new MockHttpMessageHandler(statusCode, content);
        return new HttpClient(handler);
    }

    [Fact]
    public async Task Enabled_False_Returns_Valid()
    {
        var service = new GoogleRecaptchaService(new HttpClient(), MSOptions.Create(DefaultOptions(enabled: false)));
        var result = await service.ValidateAsync("any-token");
        Assert.True(result.IsValid);
        Assert.Equal(1.0, result.Score);
    }

    [Fact]
    public async Task Score_Above_Threshold_Returns_Valid()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, """{"success": true, "score": 0.9}""");
        var service = new GoogleRecaptchaService(client, MSOptions.Create(DefaultOptions()));
        var result = await service.ValidateAsync("valid-token");
        Assert.True(result.IsValid);
        Assert.Equal(0.9, result.Score);
    }

    [Fact]
    public async Task Score_Below_Threshold_Returns_Invalid()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, """{"success": true, "score": 0.1}""");
        var service = new GoogleRecaptchaService(client, MSOptions.Create(DefaultOptions()));
        var result = await service.ValidateAsync("low-score-token");
        Assert.False(result.IsValid);
        Assert.Equal(0.1, result.Score);
    }

    [Fact]
    public async Task Http_Failure_Returns_Invalid()
    {
        var client = CreateMockHttpClient(HttpStatusCode.InternalServerError, "error");
        var service = new GoogleRecaptchaService(client, MSOptions.Create(DefaultOptions()));
        var result = await service.ValidateAsync("any-token");
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task Malformed_Json_Returns_Invalid()
    {
        var client = CreateMockHttpClient(HttpStatusCode.OK, "not-json");
        var service = new GoogleRecaptchaService(client, MSOptions.Create(DefaultOptions()));
        var result = await service.ValidateAsync("any-token");
        Assert.False(result.IsValid);
        Assert.NotNull(result.ErrorMessage);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public MockHttpMessageHandler(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content, Encoding.UTF8, "application/json")
            });
        }
    }
}
