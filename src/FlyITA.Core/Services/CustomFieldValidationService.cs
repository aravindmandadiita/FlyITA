using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;

namespace FlyITA.Core.Services;

public class CustomFieldValidationService : ICustomFieldValidationService
{
    private readonly ICustomFieldsCache _cache;
    private readonly IPCentralDataAccess _dataAccess;

    public CustomFieldValidationService(ICustomFieldsCache cache, IPCentralDataAccess dataAccess)
    {
        _cache = cache;
        _dataAccess = dataAccess;
    }

    public ValidationResult ValidateAndSaveCustomFields(CustomFieldControlCollection controls, int partyId = 0)
    {
        var result = new ValidationResult();

        // Step 1: Check required-if conditions
        foreach (var control in controls)
        {
            var requiredIf = _cache.GetCustomFieldAttribute(control.CustomFieldName, "required_if");
            if (!string.IsNullOrEmpty(requiredIf))
            {
                var parts = requiredIf.Split('=');
                if (parts.Length == 2)
                {
                    var dependentField = controls[parts[0]];
                    if (dependentField != null && dependentField.Value == parts[1])
                    {
                        control.IsRequired = true;
                    }
                }
            }
        }

        // Step 2: Validate required fields
        foreach (var control in controls)
        {
            if (control.IsRequired && string.IsNullOrWhiteSpace(control.Value) && control.CustomFieldPossibleValueID == 0)
            {
                result.AddError($"{control.UILabel} is required.");
            }
        }

        // Step 3: Save if valid
        if (result.IsValid)
        {
            foreach (var control in controls)
            {
                if (!control.WriteOnce || control.CustomFieldID == 0)
                {
                    _dataAccess.SaveCustomFieldValue(
                        control.ParticipantID,
                        control.GetCustomFieldID(),
                        control.Value,
                        control.CustomFieldPossibleValueID);
                }
            }
        }

        return result;
    }

    public void SetCustomFieldParticipantId(CustomFieldControlCollection controls, int guestParticipantId)
    {
        foreach (var control in controls)
        {
            control.ParticipantID = guestParticipantId;
        }
    }
}
