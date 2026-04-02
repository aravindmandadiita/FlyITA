using Moq;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class CustomFieldValidationServiceTests
{
    private readonly Mock<ICustomFieldsCache> _cacheMock = new();
    private readonly Mock<IPCentralDataAccess> _dataMock = new();

    private CustomFieldValidationService CreateService() => new(_cacheMock.Object, _dataMock.Object);

    [Fact]
    public void ValidateAndSave_EmptyCollection_IsValid()
    {
        var controls = new CustomFieldControlCollection();
        var result = CreateService().ValidateAndSaveCustomFields(controls);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateAndSave_RequiredField_Empty_Fails()
    {
        var controls = new CustomFieldControlCollection();
        controls.Add(new CustomFieldControlModel
        {
            CustomFieldID = 1,
            CustomFieldName = "Diet",
            IsRequired = true,
            Value = ""
        });
        _cacheMock.Setup(c => c.GetCustomFieldAttribute("Diet", "required_if")).Returns((string?)null);

        var result = CreateService().ValidateAndSaveCustomFields(controls);
        Assert.False(result.IsValid);
        Assert.Contains("Diet is required", result.Errors[0]);
    }

    [Fact]
    public void ValidateAndSave_RequiredField_Filled_Saves()
    {
        var controls = new CustomFieldControlCollection();
        controls.Add(new CustomFieldControlModel
        {
            CustomFieldID = 1,
            CustomFieldName = "Diet",
            IsRequired = true,
            Value = "Vegetarian",
            ParticipantID = 42
        });
        _cacheMock.Setup(c => c.GetCustomFieldAttribute("Diet", "required_if")).Returns((string?)null);

        var result = CreateService().ValidateAndSaveCustomFields(controls);
        Assert.True(result.IsValid);
        _dataMock.Verify(d => d.SaveCustomFieldValue(42, 1, "Vegetarian", 0), Times.Once);
    }

    [Fact]
    public void ValidateAndSave_RequiredIf_Triggers()
    {
        var controls = new CustomFieldControlCollection();
        controls.Add(new CustomFieldControlModel { CustomFieldID = 1, CustomFieldName = "HasAllergy", Value = "Yes" });
        controls.Add(new CustomFieldControlModel { CustomFieldID = 2, CustomFieldName = "AllergyDetails", Value = "", IsRequired = false });
        _cacheMock.Setup(c => c.GetCustomFieldAttribute("HasAllergy", "required_if")).Returns((string?)null);
        _cacheMock.Setup(c => c.GetCustomFieldAttribute("AllergyDetails", "required_if")).Returns("HasAllergy=Yes");

        var result = CreateService().ValidateAndSaveCustomFields(controls);
        Assert.False(result.IsValid);
        Assert.Contains("AllergyDetails is required", result.Errors[0]);
    }

    [Fact]
    public void SetCustomFieldParticipantId_Updates_All()
    {
        var controls = new CustomFieldControlCollection();
        controls.Add(new CustomFieldControlModel { CustomFieldID = 1, CustomFieldName = "A", ParticipantID = 0 });
        controls.Add(new CustomFieldControlModel { CustomFieldID = 2, CustomFieldName = "B", ParticipantID = 0 });

        CreateService().SetCustomFieldParticipantId(controls, 99);

        Assert.All(controls, c => Assert.Equal(99, c.ParticipantID));
    }
}
