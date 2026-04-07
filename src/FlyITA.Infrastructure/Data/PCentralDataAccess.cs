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

    public Task<Dictionary<string, object?>?> GetParticipantByIdAsync(int participantId)
        => GetDictionaryAsync($"api/participants/{participantId}");

    public Task<Dictionary<string, object?>?> GetPersonByIdAsync(int personId)
        => GetDictionaryAsync($"api/persons/{personId}");

    public Task<Dictionary<string, object?>?> GetPartyByParticipantIdAsync(int participantId)
        => GetDictionaryAsync($"api/participants/{participantId}/party");

    public Task<Dictionary<string, object?>?> GetProgramByIdAsync(int programId)
        => GetDictionaryAsync($"api/programs/{programId}");

    public Task<List<Dictionary<string, object?>>> GetCustomFieldValuesAsync(int participantId)
        => GetDictionaryListAsync($"api/participants/{participantId}/custom-fields");

    public async Task SaveCustomFieldValueAsync(int participantId, int customFieldId, string value, int possibleValueId)
    {
        var payload = new { customFieldId, value, possibleValueId };
        var response = await _http.PutAsJsonAsync($"api/participants/{participantId}/custom-fields", payload);
        response.EnsureSuccessStatusCode();
    }

    public Task<Dictionary<string, object?>?> GetAccommodationDetailsAsync(int participantId)
        => GetDictionaryAsync($"api/participants/{participantId}/accommodations");

    public Task<List<Dictionary<string, object?>>> GetAccommodationListAsync(int participantId)
        => GetDictionaryListAsync($"api/participants/{participantId}/accommodations/list");

    public async Task SaveAccommodationRecordAsync(int participantId, Dictionary<string, object?> data)
    {
        var response = await _http.PutAsJsonAsync($"api/participants/{participantId}/accommodations", data);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAccommodationRecordAsync(int participantId, string recordType)
    {
        var response = await _http.DeleteAsync($"api/participants/{participantId}/accommodations/{Uri.EscapeDataString(recordType)}");
        response.EnsureSuccessStatusCode();
    }

    public Task<string?> GetEmailTemplateAsync(string templateName, int programId)
        => GetStringValueAsync($"api/email-templates/{Uri.EscapeDataString(templateName)}?programId={programId}");

    public Task<string?> GetEmailBodyAsync(string templateKey, int programId)
        => GetStringValueAsync($"api/email-body/{Uri.EscapeDataString(templateKey)}?programId={programId}");

    public Task<List<Dictionary<string, object?>>> GetContactNumbersAsync(int personId)
        => GetDictionaryListAsync($"api/persons/{personId}/contacts");

    public Task<Dictionary<string, object?>?> GetPageConfigurationAsync(string pageName, string programNumber)
        => GetDictionaryAsync($"api/page-config/{Uri.EscapeDataString(pageName)}?programNumber={Uri.EscapeDataString(programNumber)}");

    public Task<Dictionary<string, object?>?> GetTransportationDetailsAsync(int participantId)
        => GetDictionaryAsync($"api/participants/{participantId}/transportation");

    private async Task<Dictionary<string, object?>?> GetDictionaryAsync(string url)
    {
        var response = await _http.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(json) ? null : DeserializeDictionary(json);
    }

    private async Task<List<Dictionary<string, object?>>> GetDictionaryListAsync(string url)
    {
        var response = await _http.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<Dictionary<string, object?>>();
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(json) ? new List<Dictionary<string, object?>>() : DeserializeDictionaryList(json);
    }

    private async Task<string?> GetStringValueAsync(string url)
    {
        var response = await _http.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(json))
            return null;

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("value", out var valueProp) ? valueProp.GetString() : null;
    }

    private static Dictionary<string, object?> DeserializeDictionary(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var dict = new Dictionary<string, object?>();
        foreach (var prop in doc.RootElement.EnumerateObject())
            dict[prop.Name] = GetJsonValue(prop.Value);
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
                dict[prop.Name] = GetJsonValue(prop.Value);
            list.Add(dict);
        }
        return list;
    }

    private static object? GetJsonValue(JsonElement element) => element.ValueKind switch
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
