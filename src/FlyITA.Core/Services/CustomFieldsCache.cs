using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Services;

public class CustomFieldsCache : ICustomFieldsCache
{
    private const string CacheKey = "CustomFieldsCache";
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public CustomFieldsCache(IMemoryCache cache, IConfiguration configuration)
    {
        _cache = cache;
        _configuration = configuration;
    }

    public int CountOfCustomFieldsInConfig()
    {
        var fields = GetCurrentCache();
        return fields?.Count ?? 0;
    }

    public bool IsCustomFieldInConfig(string cfName)
    {
        var fields = GetCurrentCache();
        return fields?.ContainsKey(cfName) ?? false;
    }

    public string? GetCustomFieldAttribute(string cfName, string cfAttrib)
    {
        var fields = GetCurrentCache();
        if (fields == null || !fields.ContainsKey(cfName))
            return null;

        var attributes = fields[cfName] as Dictionary<string, string>;
        if (attributes == null || !attributes.ContainsKey(cfAttrib))
            return null;

        return attributes[cfAttrib];
    }

    private Dictionary<string, object>? GetCurrentCache()
    {
        if (_cache.TryGetValue(CacheKey, out Dictionary<string, object>? cached))
            return cached;

        var loaded = LoadFromConfig();
        if (loaded != null)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));
            _cache.Set(CacheKey, loaded, options);
        }

        return loaded;
    }

    private Dictionary<string, object>? LoadFromConfig()
    {
        var configPath = _configuration["CustomFieldsConfigPath"];
        if (string.IsNullOrEmpty(configPath) || !File.Exists(configPath))
            return null;

        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var doc = XDocument.Load(configPath);
            var fields = doc.Descendants("CustomField");

            foreach (var field in fields)
            {
                var name = field.Attribute("name")?.Value;
                if (string.IsNullOrEmpty(name)) continue;

                var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var attr in field.Attributes())
                {
                    attributes[attr.Name.LocalName] = attr.Value;
                }

                result[name] = attributes;
            }
        }
        catch
        {
            return null;
        }

        return result;
    }
}
