using FlyITA.Core.Abstractions;
using FlyITA.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FlyITA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFlyITAInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseAccess, DatabaseAccess>();
        return services;
    }
}
