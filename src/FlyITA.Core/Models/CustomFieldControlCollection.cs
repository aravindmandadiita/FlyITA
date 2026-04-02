using System.Collections.ObjectModel;
using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Models;

public class CustomFieldControlCollection : Collection<ICustomFieldControl>
{
    public bool ContainsField(string customFieldName)
    {
        return this.Any(c => c.CustomFieldName.Equals(customFieldName, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool ContainsField(int customFieldId)
    {
        return this.Any(c => c.CustomFieldID == customFieldId);
    }

    public ICustomFieldControl? this[string name]
    {
        get => this.FirstOrDefault(c => c.CustomFieldName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
    }

    public ICustomFieldControl? ByID(int customFieldId)
    {
        return this.FirstOrDefault(c => c.CustomFieldID == customFieldId);
    }
}
