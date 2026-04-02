using FlyITA.Core.Models;

namespace FlyITA.Core.Abstractions;

public interface IPCentralDataAccess
{
    // Participant / Person
    Dictionary<string, object?>? GetParticipantById(int participantId);
    Dictionary<string, object?>? GetPersonById(int personId);
    Dictionary<string, object?>? GetPartyByParticipantId(int participantId);

    // Program
    Dictionary<string, object?>? GetProgramById(int programId);

    // Custom Fields
    List<Dictionary<string, object?>> GetCustomFieldValues(int participantId);
    void SaveCustomFieldValue(int participantId, int customFieldId, string value, int possibleValueId);

    // Accommodations
    Dictionary<string, object?>? GetAccommodationDetails(int participantId);
    List<Dictionary<string, object?>> GetAccommodationList(int participantId);
    void SaveAccommodationRecord(int participantId, Dictionary<string, object?> data);
    void DeleteAccommodationRecord(int participantId, string recordType);

    // Email Templates
    string? GetEmailTemplate(string templateName, int programId);
    string? GetEmailBody(string templateKey, int programId);

    // Contact Numbers
    List<Dictionary<string, object?>> GetContactNumbers(int personId);

    // Page Configuration
    Dictionary<string, object?>? GetPageConfiguration(string pageName, string programNumber);

    // Transportation
    Dictionary<string, object?>? GetTransportationDetails(int participantId);
}
