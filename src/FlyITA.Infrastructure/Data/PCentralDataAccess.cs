using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FlyITA.Core.Abstractions;

namespace FlyITA.Infrastructure.Data;

public class PCentralDataAccess : IPCentralDataAccess
{
    private readonly HttpClient _http;

    public PCentralDataAccess(HttpClient http)
    {
        _http = http;
    }

    // Participant / Person — single-row methods

    public Dictionary<string, object?>? GetParticipantById(int participantId)
    {
        return GetDictionary($"api/participants/{participantId}");
    }

    public Dictionary<string, object?>? GetPersonById(int personId)
    {
        return GetDictionary($"api/persons/{personId}");
    }

    public Dictionary<string, object?>? GetPartyByParticipantId(int participantId)
    {
        return GetDictionary($"api/participants/{participantId}/party");
    }

    // Program — single-row

    public Dictionary<string, object?>? GetProgramById(int programId)
    {
        return GetDictionary($"api/programs/{programId}");
    }

    // Custom Fields — multi-row + void

    public List<Dictionary<string, object?>> GetCustomFieldValues(int participantId)
    {
        return GetDictionaryList($"api/participants/{participantId}/custom-fields");
    }

    public void SaveCustomFieldValue(int participantId, int customFieldId, string value, int possibleValueId)
    {
        var payload = new { customFieldId, value, possibleValueId };
        var response = _http.PutAsJsonAsync($"api/participants/{participantId}/custom-fields", payload).ConfigureAwait(false).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }

    // Accommodations — single-row, multi-row, void

    public Dictionary<string, object?>? GetAccommodationDetails(int participantId)
    {
        return GetDictionary($"api/participants/{participantId}/accommodations");
    }

    public List<Dictionary<string, object?>> GetAccommodationList(int participantId)
    {
        return GetDictionaryList($"api/participants/{participantId}/accommodations/list");
    }

    public void SaveAccommodationRecord(int participantId, Dictionary<string, object?> data)
    {
        var response = _http.PutAsJsonAsync($"api/participants/{participantId}/accommodations", data).ConfigureAwait(false).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }

    public void DeleteAccommodationRecord(int participantId, string recordType)
    {
        var response = _http.DeleteAsync($"api/participants/{participantId}/accommodations/{Uri.EscapeDataString(recordType)}").ConfigureAwait(false).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }

    // Email Templates — string return methods

    public string? GetEmailTemplate(string templateName, int programId)
    {
        return GetStringValue($"api/email-templates/{Uri.EscapeDataString(templateName)}?programId={programId}");
    }

    public string? GetEmailBody(string templateKey, int programId)
    {
        return GetStringValue($"api/email-body/{Uri.EscapeDataString(templateKey)}?programId={programId}");
    }

    // Contact Numbers — multi-row

    public List<Dictionary<string, object?>> GetContactNumbers(int personId)
    {
        return GetDictionaryList($"api/persons/{personId}/contacts");
    }

    // Page Configuration — single-row

    public Dictionary<string, object?>? GetPageConfiguration(string pageName, string programNumber)
    {
        return GetDictionary($"api/page-config/{Uri.EscapeDataString(pageName)}?programNumber={Uri.EscapeDataString(programNumber)}");
    }

    // Transportation — single-row

    public Dictionary<string, object?>? GetTransportationDetails(int participantId)
    {
        return GetDictionary($"api/participants/{participantId}/transportation");
    }

    // Private helpers

    private Dictionary<string, object?>? GetDictionary(string url)
    {
        var response = _http.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(json))
            return null;

        return DeserializeDictionary(json);
    }

    private List<Dictionary<string, object?>> GetDictionaryList(string url)
    {
        var response = _http.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<Dictionary<string, object?>>();
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(json))
            return new List<Dictionary<string, object?>>();

        return DeserializeDictionaryList(json);
    }

    private string? GetStringValue(string url)
    {
        var response = _http.GetAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        if (string.IsNullOrEmpty(json))
            return null;

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("value", out var valueProp))
            return valueProp.GetString();

        return null;
    }

    private static Dictionary<string, object?> DeserializeDictionary(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var dict = new Dictionary<string, object?>();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            dict[prop.Name] = GetJsonValue(prop.Value);
        }
        return dict;
    }

    private static List<Dictionary<string, object?>> DeserializeDictionaryList(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var list = new List<Dictionary<string, object?>>();
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = GetJsonValue(prop.Value);
            }
            list.Add(dict);
        }
        return list;
    }

    private static object? GetJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt32(out var i) => i,
            JsonValueKind.Number when element.TryGetInt64(out var l) => l,
            JsonValueKind.Number => element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.GetRawText()
        };
    }
}
