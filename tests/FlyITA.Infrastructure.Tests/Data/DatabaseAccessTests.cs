using FlyITA.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FlyITA.Infrastructure.Tests.Data;

public class DatabaseAccessTests
{
    [Fact]
    public void Constructor_ThrowsWhenConnectionStringMissing()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(
            () => new DatabaseAccess(configuration));

        Assert.Contains("Default", exception.Message);
    }

    [Fact]
    public void Constructor_SucceedsWithValidConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "Server=test;Database=test;"
            })
            .Build();

        var access = new DatabaseAccess(configuration);

        Assert.NotNull(access);
    }
}
