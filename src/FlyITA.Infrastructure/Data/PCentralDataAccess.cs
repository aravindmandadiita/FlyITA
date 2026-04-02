using FlyITA.Core.Abstractions;

namespace FlyITA.Infrastructure.Data;

public class PCentralDataAccess : IPCentralDataAccess
{
    private readonly IDatabaseAccess _db;

    public PCentralDataAccess(IDatabaseAccess db)
    {
        _db = db;
    }

    // Participant / Person — single-row methods

    public Dictionary<string, object?>? GetParticipantById(int participantId)
    {
        return _db.ExecuteStoredProcedure("spWRASelParticipant", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    public Dictionary<string, object?>? GetPersonById(int personId)
    {
        return _db.ExecuteStoredProcedure("spWRASelPerson", new()
        {
            ["@PersonID"] = personId
        });
    }

    public Dictionary<string, object?>? GetPartyByParticipantId(int participantId)
    {
        return _db.ExecuteStoredProcedure("spWRASelPartyByParticipant", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    // Program — single-row

    public Dictionary<string, object?>? GetProgramById(int programId)
    {
        return _db.ExecuteStoredProcedure("spWRASelProgram", new()
        {
            ["@ProgramID"] = programId
        });
    }

    // Custom Fields — multi-row + void

    public List<Dictionary<string, object?>> GetCustomFieldValues(int participantId)
    {
        return _db.ExecuteStoredProcedureList("spWRASelCustomFieldValues", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    public void SaveCustomFieldValue(int participantId, int customFieldId, string value, int possibleValueId)
    {
        _db.ExecuteNonQuery("spWRAInsUpdCustomFieldValue", new()
        {
            ["@ParticipantID"] = participantId,
            ["@CustomFieldID"] = customFieldId,
            ["@Value"] = value,
            ["@PossibleValueID"] = possibleValueId
        });
    }

    // Accommodations — single-row, multi-row, void

    public Dictionary<string, object?>? GetAccommodationDetails(int participantId)
    {
        return _db.ExecuteStoredProcedure("spWRASelAccommodation", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    public List<Dictionary<string, object?>> GetAccommodationList(int participantId)
    {
        return _db.ExecuteStoredProcedureList("spWRASelAccommodationList", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    public void SaveAccommodationRecord(int participantId, Dictionary<string, object?> data)
    {
        var parameters = new Dictionary<string, object?>(data)
        {
            ["@ParticipantID"] = participantId
        };
        _db.ExecuteNonQuery("spWRAInsUpdAccommodation", parameters);
    }

    public void DeleteAccommodationRecord(int participantId, string recordType)
    {
        _db.ExecuteNonQuery("spWRADelAccommodation", new()
        {
            ["@ParticipantID"] = participantId,
            ["@RecordType"] = recordType
        });
    }

    // Email Templates — string return methods

    public string? GetEmailTemplate(string templateName, int programId)
    {
        var result = _db.ExecuteStoredProcedure("spWRASelEmailTemplate", new()
        {
            ["@TemplateName"] = templateName,
            ["@ProgramID"] = programId
        });
        return ExtractFirstColumnAsString(result);
    }

    public string? GetEmailBody(string templateKey, int programId)
    {
        var result = _db.ExecuteStoredProcedure("spWRASelEmailBody", new()
        {
            ["@TemplateKey"] = templateKey,
            ["@ProgramID"] = programId
        });
        return ExtractFirstColumnAsString(result);
    }

    // Contact Numbers — multi-row

    public List<Dictionary<string, object?>> GetContactNumbers(int personId)
    {
        return _db.ExecuteStoredProcedureList("spWRASelContactNumbers", new()
        {
            ["@PersonID"] = personId
        });
    }

    // Page Configuration — single-row

    public Dictionary<string, object?>? GetPageConfiguration(string pageName, string programNumber)
    {
        return _db.ExecuteStoredProcedure("spWRASelPageConfiguration", new()
        {
            ["@PageName"] = pageName,
            ["@ProgramNumber"] = programNumber
        });
    }

    // Transportation — single-row

    public Dictionary<string, object?>? GetTransportationDetails(int participantId)
    {
        return _db.ExecuteStoredProcedure("spWRASelTransportation", new()
        {
            ["@ParticipantID"] = participantId
        });
    }

    private static string? ExtractFirstColumnAsString(Dictionary<string, object?>? result)
    {
        if (result == null || result.Count == 0)
            return null;

        var firstValue = result.Values.FirstOrDefault();
        return firstValue?.ToString();
    }
}
