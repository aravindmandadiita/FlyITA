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
    public void GetParticipantById_CallsCorrectUrlAndReturnsData()
    {
        var json = """{"ParticipantID":42,"FirstName":"Jane","LastName":"Doe"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetParticipantById(42);

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Contains("api/participants/42", handler.LastRequest.RequestUri!.ToString());
        Assert.NotNull(result);
        Assert.Equal("Jane", result!["FirstName"]?.ToString());
        Assert.Equal(42, Convert.ToInt32(result["ParticipantID"]));
    }

    [Fact]
    public void GetParticipantById_ReturnsNull_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Null(svc.GetParticipantById(999));
    }

    [Fact]
    public void GetParticipantById_ThrowsOnServerError()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.InternalServerError, "");
        Assert.Throws<HttpRequestException>(() => svc.GetParticipantById(1));
    }

    [Fact]
    public void GetPersonById_CallsCorrectUrlAndReturnsData()
    {
        var json = """{"PersonID":10,"FirstName":"John"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetPersonById(10);

        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Contains("api/persons/10", handler.LastRequest.RequestUri!.ToString());
        Assert.Equal("John", result!["FirstName"]?.ToString());
    }

    [Fact]
    public void GetPartyByParticipantId_CallsCorrectUrl()
    {
        var json = """{"PartyID":5,"ParticipantID":7}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetPartyByParticipantId(7);

        Assert.Contains("api/participants/7/party", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(5, Convert.ToInt32(result!["PartyID"]));
    }

    [Fact]
    public void GetProgramById_CallsCorrectUrl()
    {
        var json = """{"ProgramID":100,"ProgramName":"Event"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetProgramById(100);

        Assert.Contains("api/programs/100", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Event", result!["ProgramName"]?.ToString());
    }

    // Custom Fields — multi-row + void

    [Fact]
    public void GetCustomFieldValues_CallsCorrectUrlAndReturnsList()
    {
        var json = """[{"FieldName":"Dietary","Value":"Vegetarian"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetCustomFieldValues(42);

        Assert.Contains("api/participants/42/custom-fields", handler.LastRequest!.RequestUri!.ToString());
        Assert.Single(result);
        Assert.Equal("Dietary", result[0]["FieldName"]?.ToString());
    }

    [Fact]
    public void GetCustomFieldValues_ReturnsEmptyList_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Empty(svc.GetCustomFieldValues(999));
    }

    [Fact]
    public void SaveCustomFieldValue_SendsPutToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        svc.SaveCustomFieldValue(1, 2, "value", 3);

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/1/custom-fields", handler.LastRequest.RequestUri!.ToString());
    }

    // Accommodations

    [Fact]
    public void GetAccommodationDetails_CallsCorrectUrl()
    {
        var json = """{"HotelName":"Hilton","ParticipantID":5}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetAccommodationDetails(5);

        Assert.Contains("api/participants/5/accommodations", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Hilton", result!["HotelName"]?.ToString());
    }

    [Fact]
    public void GetAccommodationList_CallsCorrectUrl()
    {
        var json = """[{"Type":"Hotel"},{"Type":"Flight"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetAccommodationList(5);

        Assert.Contains("api/participants/5/accommodations/list", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SaveAccommodationRecord_SendsPutToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        svc.SaveAccommodationRecord(5, new Dictionary<string, object?> { ["HotelName"] = "Marriott" });

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public void DeleteAccommodationRecord_SendsDeleteToCorrectUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(HttpStatusCode.NoContent, "");

        svc.DeleteAccommodationRecord(5, "Hotel");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations/Hotel", handler.LastRequest.RequestUri!.ToString());
    }

    // Email Templates — string return

    [Fact]
    public void GetEmailTemplate_CallsCorrectUrlAndExtractsValue()
    {
        var json = """{"value":"<html>Hello</html>"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetEmailTemplate("RegConfirm", 1);

        Assert.Contains("api/email-templates/RegConfirm?programId=1", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("<html>Hello</html>", result);
    }

    [Fact]
    public void GetEmailBody_CallsCorrectUrlAndExtractsValue()
    {
        var json = """{"value":"Your login is..."}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetEmailBody("LogonCreds", 2);

        Assert.Contains("api/email-body/LogonCreds?programId=2", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Your login is...", result);
    }

    [Fact]
    public void GetEmailTemplate_ReturnsNull_OnNotFound()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.NotFound, "");
        Assert.Null(svc.GetEmailTemplate("Missing", 1));
    }

    [Fact]
    public void GetEmailTemplate_ThrowsOnServerError()
    {
        var (svc, _) = CreateServiceWithHandler(HttpStatusCode.InternalServerError, "");
        Assert.Throws<HttpRequestException>(() => svc.GetEmailTemplate("Fail", 1));
    }

    // Contact Numbers — multi-row

    [Fact]
    public void GetContactNumbers_CallsCorrectUrl()
    {
        var json = """[{"Phone":"555-1234"}]""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetContactNumbers(10);

        Assert.Contains("api/persons/10/contacts", handler.LastRequest!.RequestUri!.ToString());
        Assert.Single(result);
    }

    // Page Configuration

    [Fact]
    public void GetPageConfiguration_CallsCorrectUrlWithEscapedParams()
    {
        var json = """{"RequiresAuth":true,"PageName":"profile"}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetPageConfiguration("profile", "ABC");

        Assert.Contains("api/page-config/profile?programNumber=ABC", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal(true, result!["RequiresAuth"]);
    }

    // Transportation

    [Fact]
    public void GetTransportationDetails_CallsCorrectUrl()
    {
        var json = """{"Type":"Air","ParticipantID":3}""";
        var (svc, handler) = CreateServiceWithHandler(json: json);

        var result = svc.GetTransportationDetails(3);

        Assert.Contains("api/participants/3/transportation", handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Air", result!["Type"]?.ToString());
    }

    // URL escaping

    [Fact]
    public void GetPageConfiguration_IncludesParamsInUrl()
    {
        var (svc, handler) = CreateServiceWithHandler(json: """{"PageName":"test"}""");

        svc.GetPageConfiguration("profile", "ABC123");

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
