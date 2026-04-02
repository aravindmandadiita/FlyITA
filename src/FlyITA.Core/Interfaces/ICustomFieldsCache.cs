namespace FlyITA.Core.Interfaces;

public interface ICustomFieldsCache
{
    int CountOfCustomFieldsInConfig();
    bool IsCustomFieldInConfig(string cfName);
    string? GetCustomFieldAttribute(string cfName, string cfAttrib);
}
