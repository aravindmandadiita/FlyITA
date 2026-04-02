using Moq;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class PageConfigurationServiceTests
{
    private readonly Mock<IDatabaseAccess> _dbMock = new();

    private PageConfigurationService CreateService(Dictionary<string, string?>? configPairs = null)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configPairs ?? new())
            .Build();
        return new PageConfigurationService(_dbMock.Object, config);
    }

    [Fact]
    public void ReadPageConfigSettings_Returns_DB_Values()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelPageConfiguration", It.IsAny<Dictionary<string, object?>>()))
            .Returns(new Dictionary<string, object?> { ["closeregistrationtext"] = "Closed", ["display_nav"] = true });

        var service = CreateService(new() { ["ITAProgNbr"] = "123" });
        var result = service.ReadPageConfigSettings("closed.aspx");

        Assert.Equal("Closed", result["closeregistrationtext"]);
        Assert.Equal(true, result["display_nav"]);
    }

    [Fact]
    public void ReadPageConfigSettings_Returns_Empty_When_Null()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns((Dictionary<string, object?>?)null);

        var result = CreateService().ReadPageConfigSettings("test.aspx");
        Assert.Empty(result);
    }

    [Fact]
    public void PersonMatchingEnabled_True()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelPageConfiguration", It.IsAny<Dictionary<string, object?>>()))
            .Returns(new Dictionary<string, object?> { ["personmatching"] = "true" });

        Assert.True(CreateService(new() { ["ITAProgNbr"] = "123" }).PersonMatchingEnabled());
    }

    [Fact]
    public void PersonMatchingEnabled_False_When_Missing()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns(new Dictionary<string, object?>());

        Assert.False(CreateService().PersonMatchingEnabled());
    }
}
