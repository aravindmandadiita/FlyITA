using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlyITA.Web.Components;

public class CustomFieldViewModel
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string LabelTerminator { get; set; } = ":";
    public bool IsRequired { get; set; } = true;
    public bool IsDropDown { get; set; }
    public bool WriteOnce { get; set; }
    public bool IsVisible { get; set; } = true;
    public int? MaxLength { get; set; }
    public string Value { get; set; } = string.Empty;
    public string DisplayValue { get; set; } = string.Empty;
    public List<SelectListItem> Items { get; set; } = new();
    public string? OnChange { get; set; }
    public int CustomFieldId { get; set; }
    public string CustomFieldName { get; set; } = string.Empty;
}
