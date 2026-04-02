namespace FlyITA.Web.Middleware;

public class MaintenanceModeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;

    public MaintenanceModeMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var downFile = FindDownHtml(_env.ContentRootPath);
        if (downFile != null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "text/html";
            await context.Response.SendFileAsync(downFile);
            return;
        }

        await _next(context);
    }

    private static string? FindDownHtml(string contentRoot)
    {
        var dir = new DirectoryInfo(contentRoot);
        for (var i = 0; i < 3 && dir != null; i++)
        {
            var candidate = Path.Combine(dir.FullName, "down.html");
            if (File.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}
