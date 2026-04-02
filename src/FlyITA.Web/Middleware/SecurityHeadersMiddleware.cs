using Microsoft.Extensions.Options;
using FlyITA.Web.Options;

namespace FlyITA.Web.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityOptions _options;

    public SecurityHeadersMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;
            headers["X-XSS-Protection"] = "1; mode=block";
            headers["X-Frame-Options"] = "SAMEORIGIN";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer-when-downgrade";
            headers["Permissions-Policy"] = "microphone=(),camera=(self),geolocation=(self)";
            headers["Cache-Control"] = "private,max-age=0";
            headers["Expires"] = "-1";

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
