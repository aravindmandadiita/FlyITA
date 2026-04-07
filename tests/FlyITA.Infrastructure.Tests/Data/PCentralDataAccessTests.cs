using System.Net;
using Xunit;
using FlyITA.Infrastructure.Data;

namespace FlyITA.Infrastructure.Tests.Data;

public class PCentralDataAccessTests
{
    private static (PCentralDataAccess svc, FakeHttpHandler handler) CreateServiceWithHandler(
        HttpStatusCode status = HttpStatusCode.OK, string json = "{}")
    {
        var handler = new FakeHttpHandler(status, json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        return (new PCentralDataAccess(client), handler);
    }

    // Participant / Person — single-row methods

    [Fact]
    public async Task GetParticipantByIdAsync_CallsCorrectUrlAndReturnsData()
    {
        var json = """{"ParticipantID":42,"FirstName":"Jane","LastName":"Doe"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetParticipantByIdAsync(42);

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Contains("api/participants/42", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result);
        Assert.Equal("Jane", result!["FirstName"]?.ToString());
        Assert.Equal(42, Convert.ToInt32(result["ParticipantID"]));
    }

    [Fact]
    public async Task GetParticipantByIdAsync_ReturnsNull_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Null(await svc.GetParticipantByIdAsync(999));
    }

    [Fact]
    public async Task GetParticipantByIdAsync_ThrowsOnServerError()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.InternalServerError, "");
        await Assert.ThrowsAsync<HttpRequestException>(() => svc.GetParticipantByIdAsync(1));
    }

    [Fact]
    public async Task GetPersonByIdAsync_CallsCorrectUrlAndReturnsData()
    {
        var json = """{"PersonID":10,"FirstName":"John"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetPersonByIdAsync(10);

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Contains("api/persons/10", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("John", result!["FirstName"]?.ToString());
    }

    [Fact]
    public async Task GetPartyByParticipantIdAsync_CallsCorrectUrl()
    {
        var json = """{"PartyID":5,"ParticipantID":7}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetPartyByParticipantIdAsync(7);

        Assert.Contains("api/participants/7/party", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(5, Convert.ToInt32(result!["PartyID"]));
    }

    [Fact]
    public async Task GetProgramByIdAsync_CallsCorrectUrl()
    {
        var json = """{"ProgramID":100,"ProgramName":"Event"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetProgramByIdAsync(100);

        Assert.Contains("api/programs/100", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Event", result!["ProgramName"]?.ToString());
    }

    // Custom Fields — multi-row + void

    [Fact]
    public async Task GetCustomFieldValuesAsync_CallsCorrectUrlAndReturnsList()
    {
        var json = """[{"FieldName":"Dietary","Value":"Vegetarian"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetCustomFieldValuesAsync(42);

        Assert.Contains("api/participants/42/custom-fields", handler.LastRequest!.RequestUri!.ToString());
        Assert.Single(result);
        Assert.Equal("Dietary", result[0]["FieldName"]?.ToString());
    }

    [Fact]
    public async Task GetCustomFieldValuesAsync_ReturnsEmptyList_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Empty(await svc.GetCustomFieldValuesAsync(999));
    }

    [Fact]
    public async Task SaveCustomFieldValueAsync_SendsPutToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        await svc.SaveCustomFieldValueAsync(1, 2, "value", 3);

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/1/custom-fields", handler.LastRequest.RequestUri!.ToString());
    }

    // Accommodations

    [Fact]
    public async Task GetAccommodationDetailsAsync_CallsCorrectUrl()
    {
        var json = """{"HotelName":"Hilton","ParticipantID":5}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetAccommodationDetailsAsync(5);

        Assert.Contains("api/participants/5/accommodations", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Hilton", result!["HotelName"]?.ToString());
    }

    [Fact]
    public async Task GetAccommodationListAsync_CallsCorrectUrl()
    {
        var json = """[{"Type":"Hotel"},{"Type":"Flight"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetAccommodationListAsync(5);

        Assert.Contains("api/participants/5/accommodations/list", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SaveAccommodationRecordAsync_SendsPutToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        await svc.SaveAccommodationRecordAsync(5, new Dictionary<string, object?> { ["HotelName"] = "Marriott" });

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteAccommodationRecordAsync_SendsDeleteToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        await svc.DeleteAccommodationRecordAsync(5, "Hotel");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations/Hotel", handler.LastRequest.RequestUri!.ToString());
    }

    // Email Templates — string return

    [Fact]
    public async Task GetEmailTemplateAsync_CallsCorrectUrlAndExtractsValue()
    {
        var json = """{"value":"<html>Hello</html>"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetEmailTemplateAsync("RegConfirm", 1);

        Assert.Contains("api/email-templates/RegConfirm?programId=1", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("<html>Hello</html>", result);
    }

    [Fact]
    public async Task GetEmailBodyAsync_CallsCorrectUrlAndExtractsValue()
    {
        var json = """{"value":"Your login is..."}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetEmailBodyAsync("LogonCreds", 2);

        Assert.Contains("api/email-body/LogonCreds?programId=2", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Your login is...", result);
    }

    [Fact]
    public async Task GetEmailTemplateAsync_ReturnsNull_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Null(await svc.GetEmailTemplateAsync("Missing", 1));
    }

    [Fact]
    public async Task GetEmailTemplateAsync_ThrowsOnServerError()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.InternalServerError, "");
        await Assert.ThrowsAsync<HttpRequestException>(() => svc.GetEmailTemplateAsync("Fail", 1));
    }

    // Contact Numbers — multi-row

    [Fact]
    public async Task GetContactNumbersAsync_CallsCorrectUrl()
    {
        var json = """[{"Phone":"555-1234"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetContactNumbersAsync(10);

        Assert.Contains("api/persons/10/contacts", handler.LastRequest!.RequestUri!.ToString());
        Assert.Single(result);
    }

    // Page Configuration

    [Fact]
    public async Task GetPageConfigurationAsync_CallsCorrectUrlWithEscapedParams()
    {
        var json = """{"RequiresAuth":true,"PageName":"profile"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetPageConfigurationAsync("profile", "ABC");

        Assert.Contains("api/page-config/profile?programNumber=ABC", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(true, result!["RequiresAuth"]);
    }

    // Transportation

    [Fact]
    public async Task GetTransportationDetailsAsync_CallsCorrectUrl()
    {
        var json = """{"Type":"Air","ParticipantID":3}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = await svc.GetTransportationDetailsAsync(3);

        Assert.Contains("api/participants/3/transportation", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Air", result!["Type"]?.ToString());
    }

    // URL escaping

    [Fact]
    public async Task GetPageConfigurationAsync_IncludesParamsInUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(json: """{"PageName":"test"}""");

        await svc.GetPageConfigurationAsync("profile", "ABC123");

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.Contains("api/page-config/profile", url);
        Assert.Contains("programNumber=ABC123", url);
    }
}

internal class FakeHttpHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _responseBody;

    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpHandler(HttpStatusCode statusCode, string responseBody)
    {
        _statusCode = statusCode;
        _responseBody = responseBody;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = string.IsNullOrEmpty(_responseBody)
                ? null
                : new StringContent(_responseBody, System.Text.Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
