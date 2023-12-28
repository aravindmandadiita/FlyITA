using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;


public interface ICustomFieldControl
{
    int CustomFieldID { get; set; }
    string CustomFieldName { get; set; }
    int CustomFieldPossibleValueID { get; set; }
    int ParticipantID { get; set; }
    DataTable dtCustomFieldPossibleValues { set; }
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
    int GetCustomFieldID(bool look_up_if_missing = false);
    bool HasDataSource { get; }
}


public class CustomFieldControlCollection : CollectionBase
{

    public void Add(ICustomFieldControl CustomFieldControl)
    {
        InnerList.Add(CustomFieldControl);
    }

    public void Remove(object obj)
    {
        InnerList.Remove(obj);
    }

    public new void RemoveAt(int index)
    {
        InnerList.RemoveAt(index);
    }

    public bool ContainsField(string custom_field_name)
    {
        foreach (ICustomFieldControl CFC in this.InnerList)
        {
            if (CFC.CustomFieldName.Equals(custom_field_name, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsField(int custom_field_id)
    {
        foreach (ICustomFieldControl CFC in this.InnerList)
        {
            if (CFC.CustomFieldID == custom_field_id)
            {
                return true;
            }
        }

        return false;
    }

    public ICustomFieldControl this[string name]
    {
        get
        {
            foreach (ICustomFieldControl CFC in this.InnerList)
            {
                if (CFC.CustomFieldName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return CFC;
                }
            }
            return null;
        }
    }

    public ICustomFieldControl this[int custom_field_id]
    {
        get
        {
            foreach (ICustomFieldControl CFC in this.InnerList)
            {
                if (CFC.CustomFieldID == custom_field_id)
                {
                    return CFC;
                }
            }
            return null;
        }
    }
}