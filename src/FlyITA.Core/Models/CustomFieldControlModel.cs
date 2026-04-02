using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Models;

public class CustomFieldControlModel : ICustomFieldControl
{
    public int CustomFieldID { get; set; }
    public string CustomFieldName { get; set; } = string.Empty;
    public int CustomFieldPossibleValueID { get; set; }
    public int ParticipantID { get; set; }
    public string Value { get; set; } = string.Empty;
    public string CustomFieldValue { get; set; } = string.Empty;
    public string CustomFieldText => IsDropDown ? SelectedItemText : Value;
    public bool WriteOnce { get; set; }
    public bool IsRequired { get; set; }
    public string UILabel => CustomFieldName;
    public string SelectedItemText { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public bool IsTextBox { get; set; }
    public bool AllowPostBack { get; set; }
    public bool IsDropDown { get; set; }
    public bool HasDataSource { get; set; }

    public int GetCustomFieldID(bool lookUpIfMissing = false)
    {
        return CustomFieldID;
    }
}
