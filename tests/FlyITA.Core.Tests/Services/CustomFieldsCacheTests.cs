using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class CustomFieldsCacheTests : IDisposable
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly string _tempDir;

    public CustomFieldsCacheTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"flyita-cache-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        _cache.Dispose();
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private CustomFieldsCache CreateService(string? xml = null)
    {
        if (xml != null)
        {
            var path = Path.Combine(_tempDir, "CustomFields.config");
            File.WriteAllText(path, xml);
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("CustomFieldsConfigPath", path) })
                .Build();
            return new CustomFieldsCache(_cache, config);
        }

        var emptyConfig = new ConfigurationBuilder().Build();
        return new CustomFieldsCache(_cache, emptyConfig);
    }

    [Fact]
    public void CountOfCustomFieldsInConfig_Returns_Count()
    {
        var xml = "<CustomFields><CustomField name=\"Diet\" sort=\"1\" /><CustomField name=\"Allergy\" sort=\"2\" /></CustomFields>";
        var service = CreateService(xml);
        Assert.Equal(2, service.CountOfCustomFieldsInConfig());
    }

    [Fact]
    public void IsCustomFieldInConfig_True()
    {
        var xml = "<CustomFields><CustomField name=\"Diet\" sort=\"1\" /></CustomFields>";
        Assert.True(CreateService(xml).IsCustomFieldInConfig("Diet"));
    }

    [Fact]
    public void IsCustomFieldInConfig_False()
    {
        var xml = "<CustomFields><CustomField name=\"Diet\" sort=\"1\" /></CustomFields>";
        Assert.False(CreateService(xml).IsCustomFieldInConfig("NonExistent"));
    }

    [Fact]
    public void GetCustomFieldAttribute_Found()
    {
        var xml = "<CustomFields><CustomField name=\"Diet\" sort=\"3\" required=\"true\" /></CustomFields>";
        Assert.Equal("3", CreateService(xml).GetCustomFieldAttribute("Diet", "sort"));
    }

    [Fact]
    public void GetCustomFieldAttribute_NotFound()
    {
        var xml = "<CustomFields><CustomField name=\"Diet\" sort=\"1\" /></CustomFields>";
        Assert.Null(CreateService(xml).GetCustomFieldAttribute("Diet", "missing"));
    }

    [Fact]
    public void NoConfigPath_Returns_Zero_Count()
    {
        Assert.Equal(0, CreateService().CountOfCustomFieldsInConfig());
    }
}
