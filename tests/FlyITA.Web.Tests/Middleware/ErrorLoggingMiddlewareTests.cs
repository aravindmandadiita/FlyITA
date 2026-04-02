using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;
using FlyITA.Web.Middleware;
using FlyITA.Web.Options;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace FlyITA.Web.Tests.Middleware;

public class ErrorLoggingMiddlewareTests
{
    private readonly Mock<ILogger<ErrorLoggingMiddleware>> _loggerMock = new();
    private readonly Mock<IDatabaseAccess> _dbMock = new();
    private readonly Mock<IContextManager> _contextMock = new();

    private HttpContext CreateContext(bool isClientFacing = false)
    {
        var context = new DefaultHttpContext();
        var services = new ServiceCollection();
        services.AddSingleton(_dbMock.Object);
        services.AddSingleton(_contextMock.Object);
        services.AddSingleton(MSOptions.Create(new EnvironmentOptions { IsClientFacing = isClientFacing }));
        services.AddSingleton(MSOptions.Create(new ErrorLoggingOptions()));
        services.AddMemoryCache();

        context.RequestServices = services.BuildServiceProvider();
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Fact]
    public async Task NoException_PassesThrough()
    {
        var middleware = new ErrorLoggingMiddleware(
            next: _ => Task.CompletedTask,
            _loggerMock.Object);

        var context = CreateContext();
        await middleware.InvokeAsync(context);

        _dbMock.Verify(d => d.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()), Times.Never);
    }

    [Fact]
    public async Task Exception_LogsToDatabase()
    {
        _contextMock.SetupGet(c => c.RealUserID).Returns(42);

        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new InvalidOperationException("Test error"),
            _loggerMock.Object);

        var context = CreateContext();
        await middleware.InvokeAsync(context);

        _dbMock.Verify(d => d.ExecuteNonQuery(
            "spInsLogMessage",
            It.Is<Dictionary<string, object?>>(p =>
                (int)p["@SystemID"]! == 926 &&
                (int)p["@UserID"]! == 42 &&
                ((string)p["@ErrorMessage"]!).Contains("Test error"))),
            Times.Once);
    }

    [Fact]
    public async Task Exception_Returns500()
    {
        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new Exception("fail"),
            _loggerMock.Object);

        var context = CreateContext();
        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task ClientFacing_ReturnsGenericMessage()
    {
        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new Exception("secret details"),
            _loggerMock.Object);

        var context = CreateContext(isClientFacing: true);
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.DoesNotContain("secret details", body);
        Assert.Contains("unexpected error", body);
    }

    [Fact]
    public async Task Internal_ReturnsDetailedError()
    {
        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new Exception("detailed info"),
            _loggerMock.Object);

        var context = CreateContext(isClientFacing: false);
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains("detailed info", body);
    }

    [Fact]
    public async Task DbLoggingFailure_IsSwallowed()
    {
        _dbMock.Setup(d => d.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
            .Throws(new Exception("DB down"));

        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new Exception("app error"),
            _loggerMock.Object);

        var context = CreateContext();
        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task ErrorFrequency_Increments()
    {
        var middleware = new ErrorLoggingMiddleware(
            next: _ => throw new Exception("error"),
            _loggerMock.Object);

        var context = CreateContext();
        await middleware.InvokeAsync(context);

        _dbMock.Verify(d => d.ExecuteNonQuery(
            "spInsLogMessage",
            It.Is<Dictionary<string, object?>>(p =>
                (int)p["@Freq5Min"]! == 1 &&
                (int)p["@Freq30Min"]! == 1)),
            Times.Once);
    }
}
