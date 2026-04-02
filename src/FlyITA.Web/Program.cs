using Microsoft.Extensions.Options;
using FlyITA.Core;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;
using FlyITA.Infrastructure;
using FlyITA.Web.Authentication;
using FlyITA.Web.Middleware;
using FlyITA.Web.Options;
using FlyITA.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Strongly-typed options — existing
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.SectionName));
builder.Services.Configure<AppSessionOptions>(builder.Configuration.GetSection(AppSessionOptions.SectionName));

// Strongly-typed options — Core (Phase 3)
builder.Services.Configure<EnvironmentOptions>(builder.Configuration.GetSection(EnvironmentOptions.SectionName));
builder.Services.Configure<NavigationOptions>(builder.Configuration.GetSection(NavigationOptions.SectionName));
builder.Services.Configure<ProgramOptions>(builder.Configuration.GetSection(ProgramOptions.SectionName));
builder.Services.Configure<GuestOptions>(builder.Configuration.GetSection(GuestOptions.SectionName));
builder.Services.Configure<DisplayOptions>(builder.Configuration.GetSection(DisplayOptions.SectionName));

// Strongly-typed options — Web (Phase 3)
builder.Services.Configure<LoginRedirectOptions>(builder.Configuration.GetSection(LoginRedirectOptions.SectionName));
builder.Services.Configure<ErrorLoggingOptions>(builder.Configuration.GetSection(ErrorLoggingOptions.SectionName));
builder.Services.Configure<PageOptions>(builder.Configuration.GetSection(PageOptions.SectionName));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.Configure<SecureIframeOptions>(builder.Configuration.GetSection(SecureIframeOptions.SectionName));
builder.Services.Configure<WeatherOptions>(builder.Configuration.GetSection(WeatherOptions.SectionName));

// Strongly-typed options — Core (Phase 4)
builder.Services.Configure<ExternalServicesOptions>(builder.Configuration.GetSection(ExternalServicesOptions.SectionName));
builder.Services.Configure<RecaptchaOptions>(builder.Configuration.GetSection(RecaptchaOptions.SectionName));

// 1. Core services (TryAdd null defaults for IDatabaseAccess, IEnvironmentService, ISmtpClient)
builder.Services.AddFlyITACore();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IContextManager, ContextManager>();

// 2. Infrastructure (overrides Core's null IDatabaseAccess with real SqlClient impl)
builder.Services.AddFlyITAInfrastructure();

// 3. Web services (override Core's null defaults for environment and SMTP)
builder.Services.AddSingleton<IEnvironmentService, EnvironmentService>();
builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();

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

// Authentication — SAML handler bound from config
builder.Services.AddAuthentication("Saml")
    .AddScheme<SamlOptions, SamlAuthenticationHandler>("Saml", options =>
    {
        builder.Configuration.GetSection(SamlOptions.SectionName).Bind(options);
    });
builder.Services.AddAuthorization();

// Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

// 1. Maintenance mode (first — short-circuits if down.html exists)
app.UseMiddleware<MaintenanceModeMiddleware>();

// 2. Error logging (replaces UseExceptionHandler — logs to DB + returns appropriate response)
app.UseMiddleware<ErrorLoggingMiddleware>();

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

// 7. Auth
app.UseAuthentication();
app.UseAuthorization();

// 8. Routing
app.MapRazorPages();
app.MapHealthChecks("/health");

app.Run();

// Make Program accessible to WebApplicationFactory
public partial class Program { }
