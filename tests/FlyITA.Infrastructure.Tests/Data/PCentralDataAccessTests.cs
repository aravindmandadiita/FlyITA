using System.Net;
using System.Text.Json;
using Xunit;
using FlyITA.Infrastructure.Data;

namespace FlyITA.Infrastructure.Tests.Data;

public class PCentralDataAccessTests
{
    private static HttpClient CreateClient(HttpStatusCode status, string json)
    {
        var handler = new FakeHttpHandler(status, json);
        return new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
    }

    private static PCentralDataAccess CreateService(HttpStatusCode status = HttpStatusCode.OK, string json = "{}")
    {
        return new PCentralDataAccess(CreateClient(status, json));
    }

    // Participant / Person — single-row methods

    [Fact]
    public void GetParticipantById_ReturnsDeserializedDictionary()
    {
        var json = """{"ParticipantID":42,"FirstName":"Jane","LastName":"Doe"}""";
        var svc = CreateService(json: json);

        var result = svc.GetParticipantById(42);

        Assert.NotNull(result);
        Assert.Equal("Jane", result!["FirstName"]?.ToString());
        Assert.Equal(42, Convert.ToInt32(result["ParticipantID"]));
    }

    [Fact]
    public void GetParticipantById_ReturnsNull_OnNotFound()
    {
        var svc = CreateService(HttpStatusCode.NotFound, "");

        var result = svc.GetParticipantById(999);
        Assert.Null(result);
    }

    [Fact]
    public void GetPersonById_ReturnsDeserializedDictionary()
    {
        var json = """{"PersonID":10,"FirstName":"John"}""";
        var svc = CreateService(json: json);

        var result = svc.GetPersonById(10);

        Assert.NotNull(result);
        Assert.Equal("John", result!["FirstName"]?.ToString());
    }

    [Fact]
    public void GetPartyByParticipantId_ReturnsDeserializedDictionary()
    {
        var json = """{"PartyID":5,"ParticipantID":7}""";
        var svc = CreateService(json: json);

        var result = svc.GetPartyByParticipantId(7);

        Assert.NotNull(result);
        Assert.Equal(5, Convert.ToInt32(result!["PartyID"]));
    }

    [Fact]
    public void GetProgramById_ReturnsDeserializedDictionary()
    {
        var json = """{"ProgramID":100,"ProgramName":"Event"}""";
        var svc = CreateService(json: json);

        var result = svc.GetProgramById(100);

        Assert.NotNull(result);
        Assert.Equal("Event", result!["ProgramName"]?.ToString());
    }

    // Custom Fields — multi-row + void

    [Fact]
    public void GetCustomFieldValues_ReturnsDeserializedList()
    {
        var json = """[{"FieldName":"Dietary","Value":"Vegetarian"}]""";
        var svc = CreateService(json: json);

        var result = svc.GetCustomFieldValues(42);

        Assert.Single(result);
        Assert.Equal("Dietary", result[0]["FieldName"]?.ToString());
    }

    [Fact]
    public void GetCustomFieldValues_ReturnsEmptyList_OnNotFound()
    {
        var svc = CreateService(HttpStatusCode.NotFound, "");

        var result = svc.GetCustomFieldValues(999);
        Assert.Empty(result);
    }

    [Fact]
    public void SaveCustomFieldValue_SendsPutRequest()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NoContent, "");
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        var svc = new PCentralDataAccess(client);

        svc.SaveCustomFieldValue(1, 2, "value", 3);

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/1/custom-fields", handler.LastRequest.RequestUri!.ToString());
    }

    // Accommodations

    [Fact]
    public void GetAccommodationDetails_ReturnsDeserializedDictionary()
    {
        var json = """{"HotelName":"Hilton","ParticipantID":5}""";
        var svc = CreateService(json: json);

        var result = svc.GetAccommodationDetails(5);

        Assert.NotNull(result);
        Assert.Equal("Hilton", result!["HotelName"]?.ToString());
    }

    [Fact]
    public void GetAccommodationList_ReturnsDeserializedList()
    {
        var json = """[{"Type":"Hotel"},{"Type":"Flight"}]""";
        var svc = CreateService(json: json);

        var result = svc.GetAccommodationList(5);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SaveAccommodationRecord_SendsPutRequest()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NoContent, "");
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        var svc = new PCentralDataAccess(client);

        svc.SaveAccommodationRecord(5, new Dictionary<string, object?> { ["HotelName"] = "Marriott" });

        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations", handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public void DeleteAccommodationRecord_SendsDeleteRequest()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.NoContent, "");
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        var svc = new PCentralDataAccess(client);

        svc.DeleteAccommodationRecord(5, "Hotel");

        Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
        Assert.Contains("api/participants/5/accommodations/Hotel", handler.LastRequest.RequestUri!.ToString());
    }

    // Email Templates — string return

    [Fact]
    public void GetEmailTemplate_ExtractsValueField()
    {
        var json = """{"value":"<html>Hello</html>"}""";
        var svc = CreateService(json: json);

        var result = svc.GetEmailTemplate("RegConfirm", 1);

        Assert.Equal("<html>Hello</html>", result);
    }

    [Fact]
    public void GetEmailBody_ExtractsValueField()
    {
        var json = """{"value":"Your login is..."}""";
        var svc = CreateService(json: json);

        var result = svc.GetEmailBody("LogonCreds", 2);

        Assert.Equal("Your login is...", result);
    }

    [Fact]
    public void GetEmailTemplate_ReturnsNull_OnNotFound()
    {
        var svc = CreateService(HttpStatusCode.NotFound, "");

        var result = svc.GetEmailTemplate("Missing", 1);
        Assert.Null(result);
    }

    // Contact Numbers — multi-row

    [Fact]
    public void GetContactNumbers_ReturnsDeserializedList()
    {
        var json = """[{"Phone":"555-1234"}]""";
        var svc = CreateService(json: json);

        var result = svc.GetContactNumbers(10);

        Assert.Single(result);
        Assert.Equal("555-1234", result[0]["Phone"]?.ToString());
    }

    // Page Configuration

    [Fact]
    public void GetPageConfiguration_ReturnsDeserializedDictionary()
    {
        var json = """{"RequiresAuth":true,"PageName":"profile"}""";
        var svc = CreateService(json: json);

        var result = svc.GetPageConfiguration("profile", "ABC");

        Assert.NotNull(result);
        Assert.Equal(true, result!["RequiresAuth"]);
    }

    // Transportation

    [Fact]
    public void GetTransportationDetails_ReturnsDeserializedDictionary()
    {
        var json = """{"Type":"Air","ParticipantID":3}""";
        var svc = CreateService(json: json);

        var result = svc.GetTransportationDetails(3);

        Assert.NotNull(result);
        Assert.Equal("Air", result!["Type"]?.ToString());
    }

    // Verify URL construction

    [Fact]
    public void GetEmailTemplate_ConstructsCorrectUrl()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, """{"value":"test"}""");
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        var svc = new PCentralDataAccess(client);

        svc.GetEmailTemplate("Confirm", 5);

        Assert.Contains("api/email-templates/Confirm?programId=5", handler.LastRequest!.RequestUri!.ToString());
    }

    [Fact]
    public void GetPageConfiguration_ConstructsCorrectUrl()
    {
        var handler = new FakeHttpHandler(HttpStatusCode.OK, """{"PageName":"test"}""");
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5100") };
        var svc = new PCentralDataAccess(client);

        svc.GetPageConfiguration("profile", "ABC");

        Assert.Contains("api/page-config/profile?programNumber=ABC", handler.LastRequest!.RequestUri!.ToString());
    }
}

/// <summary>
/// Fake HTTP handler for testing — returns a canned response and captures the last request.
/// </summary>
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
