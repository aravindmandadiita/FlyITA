using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;
using FlyITA.Infrastructure.Data;
using FlyITA.Infrastructure.Services;

namespace FlyITA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFlyITAInfrastructure(this IServiceCollection services)
    {
        // Data access
        services.AddScoped<IDatabaseAccess, DatabaseAccess>();
        services.AddScoped<IPCentralDataAccess, PCentralDataAccess>();

        // WCF SOAP service clients
        services.AddScoped<ICardTokenService>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<ExternalServicesOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<WcfCardTokenService>>();
            return new WcfCardTokenService(opts.CardTokenServiceUrl, opts.ServiceTimeoutSeconds, logger);
        });
        services.AddScoped<IPerformanceCentralClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<ExternalServicesOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<WcfPerformanceCentralClient>>();
            return new WcfPerformanceCentralClient(opts.PerformanceCentralServiceUrl, opts.ServiceTimeoutSeconds, logger);
        });
        services.AddScoped<ICardProcessService>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<ExternalServicesOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<WcfCardProcessService>>();
            return new WcfCardProcessService(opts.CardProcessServiceUrl, opts.ServiceTimeoutSeconds, logger);
        });

        // reCAPTCHA service (HTTP client)
        services.AddHttpClient<ICaptchaService, GoogleRecaptchaService>();

        return services;
    }
}
