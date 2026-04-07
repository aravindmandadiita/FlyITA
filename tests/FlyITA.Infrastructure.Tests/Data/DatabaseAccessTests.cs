using FlyITA.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FlyITA.Infrastructure.Tests.Data;

public class DatabaseAccessTests
{
    [Fact]
    public void Constructor_AcceptsMissingConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        var access = new DatabaseAccess(configuration);

        Assert.NotNull(access);
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
