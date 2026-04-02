using Moq;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Options;
using FlyITA.Core.Services;

using MSOptions = Microsoft.Extensions.Options.Options;

namespace FlyITA.Core.Tests.Services;

public class NavigationServiceTests
{
    private readonly Mock<IContextManager> _contextMock = new();

    private NavigationService CreateService(NavigationOptions? options = null)
    {
        var opts = MSOptions.Create(options ?? new NavigationOptions());
        return new NavigationService(opts, _contextMock.Object);
    }

    [Fact]
    public void Navigate_Returns_Target()
    {
        var service = CreateService();
        Assert.Equal("home.aspx", service.Navigate("home.aspx"));
    }

    [Fact]
    public void SetNextPage_And_GetCurrentPage_RoundTrips()
    {
        _contextMock.SetupProperty(c => c.NextPage);
        var service = CreateService();

        service.SetNextPage("profile.aspx");
        Assert.Equal("profile.aspx", service.GetCurrentPage());
    }

    [Fact]
    public void ReadNavConfigValues_Returns_RegistrationFlow_Step()
    {
        var options = new NavigationOptions
        {
            RegistrationFlow = new()
            {
                new RegistrationStepConfig { Name = "profile", Level = 1, Url = "transportation.aspx" },
                new RegistrationStepConfig { Name = "transportation", Level = 2, Url = "accommodations.aspx" }
            }
        };
        var service = CreateService(options);
        var values = service.ReadNavConfigValues("profile");

        Assert.Equal("transportation.aspx", values["default"]);
    }

    [Fact]
    public void ReadNavConfigValues_Returns_Empty_For_Unknown_Section()
    {
        var service = CreateService();
        var values = service.ReadNavConfigValues("nonexistent");
        Assert.Empty(values);
    }

    [Fact]
    public void GetNextPageByPage_Uses_NavOption()
    {
        var options = new NavigationOptions
        {
            RegistrationFlow = new()
            {
                new RegistrationStepConfig { Name = "profile", Level = 1, Url = "transportation.aspx" }
            }
        };
        var service = CreateService(options);

        var page = service.GetNextPageByPage("profile");
        Assert.Equal("transportation.aspx", page);
    }
}
