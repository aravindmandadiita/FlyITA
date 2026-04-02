using Microsoft.Extensions.Options;
using FlyITA.Core.Options;
using FlyITA.Web.Services;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace FlyITA.Web.Tests.Services;

public class EnvironmentServiceTests
{
    [Theory]
    [InlineData("CDT", false, "Default")]
    [InlineData("PR", true, "Production")]
    [InlineData("UAT", false, "Staging")]
    public void ReadsOptionsCorrectly(string role, bool isClientFacing, string connStringName)
    {
        var options = MSOptions.Create(new EnvironmentOptions
        {
            Role = role,
            IsClientFacing = isClientFacing,
            ConnectionStringName = connStringName
        });

        var service = new EnvironmentService(options);

        Assert.Equal(role, service.Role);
        Assert.Equal(isClientFacing, service.IsClientFacing);
        Assert.Equal(connStringName, service.GetConnectionStringName());
    }

    [Fact]
    public void DefaultOptions_ReturnsCDT()
    {
        var service = new EnvironmentService(MSOptions.Create(new EnvironmentOptions()));

        Assert.Equal("CDT", service.Role);
        Assert.False(service.IsClientFacing);
        Assert.Equal("Default", service.GetConnectionStringName());
    }
}
