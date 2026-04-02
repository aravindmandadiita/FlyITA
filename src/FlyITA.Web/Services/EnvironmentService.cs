using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;

namespace FlyITA.Web.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly EnvironmentOptions _options;

    public EnvironmentService(IOptions<EnvironmentOptions> options)
    {
        _options = options.Value;
    }

    public string Role => _options.Role;
    public bool IsClientFacing => _options.IsClientFacing;
    public string GetConnectionStringName() => _options.ConnectionStringName;
}
