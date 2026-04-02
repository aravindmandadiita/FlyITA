using Moq;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class NavigationServiceTests : IDisposable
{
    private readonly Mock<IContextManager> _contextMock = new();
    private readonly string _tempDir;

    public NavigationServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"flyita-nav-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private NavigationService CreateService(string? navXml = null, Dictionary<string, string?>? extraConfig = null)
    {
        var configPairs = new List<KeyValuePair<string, string?>>();
        if (navXml != null)
        {
            var path = Path.Combine(_tempDir, "Navigation.config");
            File.WriteAllText(path, navXml);
            configPairs.Add(new("NavigationConfigPath", path));
        }
        if (extraConfig != null)
            configPairs.AddRange(extraConfig);

        var config = new ConfigurationBuilder().AddInMemoryCollection(configPairs).Build();
        return new NavigationService(config, _contextMock.Object);
    }

    [Fact]
    public void ReadNavConfigValues_Reads_XML()
    {
        var xml = "<nav><profile default=\"transportation.aspx\" cruise=\"cruise.aspx\" /></nav>";
        var service = CreateService(xml);
        var values = service.ReadNavConfigValues("profile");

        Assert.Equal("transportation.aspx", values["default"]);
        Assert.Equal("cruise.aspx", values["cruise"]);
    }

    [Fact]
    public void GetRole_Returns_Config_Value()
    {
        var service = CreateService(extraConfig: new() { ["Role"] = "PR" });
        Assert.Equal("PR", service.GetRole());
    }

    [Fact]
    public void GetRole_Returns_CDT_Default()
    {
        var service = CreateService();
        Assert.Equal("CDT", service.GetRole());
    }

    [Fact]
    public void Navigate_Returns_Target()
    {
        var service = CreateService();
        Assert.Equal("home.aspx", service.Navigate("home.aspx"));
    }
}
