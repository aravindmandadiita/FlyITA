using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface ICustomFieldValidationService
{
    ValidationResult ValidateAndSaveCustomFields(CustomFieldControlCollection controls, int partyId = 0);
    void SetCustomFieldParticipantId(CustomFieldControlCollection controls, int guestParticipantId);
}
