using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface ICustomFieldValidationService
{
    Task<ValidationResult> ValidateAndSaveCustomFieldsAsync(CustomFieldControlCollection controls, int partyId = 0);
    void SetCustomFieldParticipantId(CustomFieldControlCollection controls, int guestParticipantId);
}
