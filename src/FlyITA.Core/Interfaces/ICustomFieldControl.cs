namespace FlyITA.Core.Interfaces;

public interface ICustomFieldControl
{
    int CustomFieldID { get; set; }
    string CustomFieldName { get; set; }
    int CustomFieldPossibleValueID { get; set; }
    int ParticipantID { get; set; }
    string Value { get; set; }
    string CustomFieldValue { get; set; }
    string CustomFieldText { get; }
    bool WriteOnce { get; set; }
    bool IsRequired { get; set; }
    string UILabel { get; }
    string SelectedItemText { get; set; }
    bool IsVisible { get; set; }
    bool IsTextBox { get; set; }
    bool AllowPostBack { get; set; }
    bool IsDropDown { get; set; }
    bool HasDataSource { get; }
    int GetCustomFieldID(bool lookUpIfMissing = false);
}
