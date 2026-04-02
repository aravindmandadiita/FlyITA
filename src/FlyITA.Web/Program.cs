using Microsoft.Extensions.Options;
using FlyITA.Web.Middleware;
using FlyITA.Web.Options;

var builder = WebApplication.CreateBuilder(args);

// Strongly-typed options
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.SectionName));
builder.Services.Configure<AppSessionOptions>(builder.Configuration.GetSection(AppSessionOptions.SectionName));

// Session
var sessionTimeout = builder.Configuration.GetValue<int>("Session:TimeoutMinutes", 20);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

// Health checks
builder.Services.AddHealthChecks();

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

// 4. Security headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// 5. Static files
app.UseStaticFiles();

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
