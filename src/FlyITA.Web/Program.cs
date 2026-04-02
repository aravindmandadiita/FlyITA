using Microsoft.Extensions.Options;
using FlyITA.Web.Middleware;
using FlyITA.Web.Options;

var builder = WebApplication.CreateBuilder(args);

// Strongly-typed options
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.SectionName));
builder.Services.Configure<AppSessionOptions>(builder.Configuration.GetSection(AppSessionOptions.SectionName));

// Session (uses AppSessionOptions for timeout)
var appSessionOptions = builder.Configuration
    .GetSection(AppSessionOptions.SectionName)
    .Get<AppSessionOptions>() ?? new AppSessionOptions();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(appSessionOptions.TimeoutMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

// Health checks
builder.Services.AddHealthChecks();

// Auth (placeholder — no scheme configured yet, services registered for middleware)
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// 1. Maintenance mode (first — short-circuits if down.html exists)
app.UseMiddleware<MaintenanceModeMiddleware>();

// 2. Exception handling
app.UseExceptionHandler("/Error");

// 3. HTTPS redirection (driven by config)
var securityOptions = app.Services.GetRequiredService<IOptions<SecurityOptions>>().Value;
if (securityOptions.RequireHttps)
    app.UseHttpsRedirection();

// 4. Security headers (scoped to non-static responses)
app.UseMiddleware<SecurityHeadersMiddleware>();

// 5. Static files (with browser caching for assets)
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000";
        ctx.Context.Response.Headers.Remove("Expires");
    }
});

// 6. Session
app.UseSession();

// 7. Cookie policy (configured via session cookie options above)

// 8. Auth placeholder
app.UseAuthentication();
app.UseAuthorization();

// 9. Routing
app.MapRazorPages();
app.MapHealthChecks("/health");

app.Run();

// Make Program accessible to WebApplicationFactory
public partial class Program { }
