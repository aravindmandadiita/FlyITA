namespace FlyITA.Core.Abstractions;

public interface IPCentralDataAccess
{
    // Participant / Person
    Task<Dictionary<string, object?>?> GetParticipantByIdAsync(int participantId);
    Task<Dictionary<string, object?>?> GetPersonByIdAsync(int personId);
    Task<Dictionary<string, object?>?> GetPartyByParticipantIdAsync(int participantId);

    // Program
    Task<Dictionary<string, object?>?> GetProgramByIdAsync(int programId);

    // Custom Fields
    Task<List<Dictionary<string, object?>>> GetCustomFieldValuesAsync(int participantId);
    Task SaveCustomFieldValueAsync(int participantId, int customFieldId, string value, int possibleValueId);

    // Accommodations
    Task<Dictionary<string, object?>?> GetAccommodationDetailsAsync(int participantId);
    Task<List<Dictionary<string, object?>>> GetAccommodationListAsync(int participantId);
    Task SaveAccommodationRecordAsync(int participantId, Dictionary<string, object?> data);
    Task DeleteAccommodationRecordAsync(int participantId, string recordType);

    // Email Templates
    Task<string?> GetEmailTemplateAsync(string templateName, int programId);
    Task<string?> GetEmailBodyAsync(string templateKey, int programId);

    // Contact Numbers
    Task<List<Dictionary<string, object?>>> GetContactNumbersAsync(int personId);

    // Page Configuration
    Task<Dictionary<string, object?>?> GetPageConfigurationAsync(string pageName, string programNumber);

    // Transportation
    Task<Dictionary<string, object?>?> GetTransportationDetailsAsync(int participantId);
}
