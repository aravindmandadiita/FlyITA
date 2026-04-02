using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;
using FlyITA.Web.Options;

namespace FlyITA.Web.Middleware;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        try
        {
            var dbAccess = context.RequestServices.GetService<IDatabaseAccess>();
            var contextManager = context.RequestServices.GetService<IContextManager>();
            var envOptions = context.RequestServices.GetService<IOptions<EnvironmentOptions>>()?.Value;
            var errorOptions = context.RequestServices.GetService<IOptions<ErrorLoggingOptions>>()?.Value
                ?? new ErrorLoggingOptions();
            var cache = context.RequestServices.GetService<IMemoryCache>();

            // Increment error frequency counters
            var frequencies = IncrementFrequencies(cache);

            // Log to database
            if (dbAccess != null)
            {
                try
                {
                    var errorMessage = $"{exception.GetType().FullName}: {exception.Message}\n{exception.StackTrace}";
                    var chunks = ChunkMessage(errorMessage, errorOptions.MaxMessageChunkSize);
                    var realUserId = contextManager?.RealUserID ?? 0;

                    var parameters = new Dictionary<string, object?>
                    {
                        ["@SystemID"] = errorOptions.SystemId,
                        ["@UserID"] = realUserId,
                        ["@ErrorMessage"] = chunks.Count > 0 ? chunks[0] : errorMessage,
                        ["@ErrorMessage2"] = chunks.Count > 1 ? chunks[1] : null,
                        ["@ErrorMessage3"] = chunks.Count > 2 ? chunks[2] : null,
                        ["@Freq5Min"] = frequencies.Freq5Min,
                        ["@Freq30Min"] = frequencies.Freq30Min,
                        ["@Freq1Hour"] = frequencies.Freq1Hour,
                        ["@Freq2Hour"] = frequencies.Freq2Hour
                    };

                    dbAccess.ExecuteNonQuery("spInsLogMessage", parameters);
                }
                catch (Exception dbEx)
                {
                    _logger.LogWarning(dbEx, "Failed to log error to database");
                }
            }

            // Write error response — only if headers haven't been sent yet
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/html";

                var isClientFacing = envOptions?.IsClientFacing ?? false;
                var responseMessage = isClientFacing
                    ? errorOptions.ClientFacingErrorMessage
                    : $"{errorOptions.InternalErrorMessage}\n\n{exception}";

                await context.Response.WriteAsync(responseMessage);
            }
            else
            {
                _logger.LogWarning("Response already started — cannot write error page. Aborting connection.");
                context.Abort();
            }
        }
        catch (Exception handlerEx)
        {
            _logger.LogWarning(handlerEx, "Error in error handling middleware");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An error occurred.");
        }
    }

    private static List<string> ChunkMessage(string message, int chunkSize)
    {
        var chunks = new List<string>();
        for (int i = 0; i < message.Length; i += chunkSize)
        {
            chunks.Add(message.Substring(i, Math.Min(chunkSize, message.Length - i)));
        }
        return chunks;
    }

    private static ErrorFrequencies IncrementFrequencies(IMemoryCache? cache)
    {
        if (cache == null) return new ErrorFrequencies();

        return new ErrorFrequencies
        {
            Freq5Min = IncrementCounter(cache, "error_freq_5m", TimeSpan.FromMinutes(5)),
            Freq30Min = IncrementCounter(cache, "error_freq_30m", TimeSpan.FromMinutes(30)),
            Freq1Hour = IncrementCounter(cache, "error_freq_1h", TimeSpan.FromHours(1)),
            Freq2Hour = IncrementCounter(cache, "error_freq_2h", TimeSpan.FromHours(2))
        };
    }

    private static int IncrementCounter(IMemoryCache cache, string key, TimeSpan window)
    {
        var count = cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = window;
            return new CounterHolder();
        })!;
        return Interlocked.Increment(ref count.Value);
    }

    private class CounterHolder
    {
        public int Value;
    }

    private record ErrorFrequencies
    {
        public int Freq5Min { get; init; }
        public int Freq30Min { get; init; }
        public int Freq1Hour { get; init; }
        public int Freq2Hour { get; init; }
    }
}
