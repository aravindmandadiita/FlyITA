using FlyITA.Web.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FlyITA.Web.Tests.Components;

public class CustomFieldViewComponentTests
{
    private readonly CustomFieldViewComponent _component = new();

    [Fact]
    public void Invoke_TextBox_Required_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf1", FieldName = "CustomField1",
            Label = "First Name", IsRequired = true
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.True(vm.IsRequired);
        Assert.False(vm.IsDropDown);
        Assert.False(vm.WriteOnce);
    }

    [Fact]
    public void Invoke_TextBox_Optional_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf2", FieldName = "CustomField2",
            Label = "Middle Name", IsRequired = false
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.False(vm.IsRequired);
    }

    [Fact]
    public void Invoke_DropDown_Required_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf3", FieldName = "CustomField3",
            Label = "Country", IsDropDown = true, IsRequired = true,
            Items = new List<SelectListItem>
            {
                new("Select", ""),
                new("USA", "US"),
                new("Canada", "CA")
            }
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.True(vm.IsDropDown);
        Assert.True(vm.IsRequired);
        Assert.Equal(3, vm.Items.Count);
    }

    [Fact]
    public void Invoke_DropDown_Optional_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf4", IsDropDown = true, IsRequired = false,
            Items = new List<SelectListItem> { new("Option", "1") }
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.True(vm.IsDropDown);
        Assert.False(vm.IsRequired);
    }

    [Fact]
    public void Invoke_WriteOnce_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf5", WriteOnce = true,
            DisplayValue = "John Doe", Label = "Name"
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.True(vm.WriteOnce);
        Assert.Equal("John Doe", vm.DisplayValue);
    }

    [Fact]
    public void Invoke_NotVisible_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel { FieldId = "cf6", IsVisible = false };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.False(vm.IsVisible);
    }

    [Fact]
    public void Invoke_WithMaxLength_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf7", MaxLength = 50, Label = "City"
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.Equal(50, vm.MaxLength);
    }

    [Fact]
    public void Invoke_WithOnChange_ReturnsViewWithModel()
    {
        var model = new CustomFieldViewModel
        {
            FieldId = "cf8", OnChange = "handleChange(this)"
        };

        var result = _component.Invoke(model);
        var viewResult = Assert.IsType<ViewViewComponentResult>(result);
        var vm = Assert.IsType<CustomFieldViewModel>(viewResult.ViewData!.Model);
        Assert.Equal("handleChange(this)", vm.OnChange);
    }
}
