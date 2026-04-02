using Xunit;
using Moq;
using FlyITA.Core.Abstractions;
using FlyITA.Infrastructure.Data;

namespace FlyITA.Infrastructure.Tests.Data;

public class PCentralDataAccessTests
{
    private readonly Mock<IDatabaseAccess> _dbMock = new();
    private PCentralDataAccess CreateService() => new(_dbMock.Object);

    // Participant / Person — single-row methods

    [Fact]
    public void GetParticipantById_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["Name"] = "Test" };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelParticipant",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 42)))
            .Returns(expected);

        var result = CreateService().GetParticipantById(42);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPersonById_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["FirstName"] = "John" };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelPerson",
            It.Is<Dictionary<string, object?>>(p => (int)p["@PersonID"]! == 10)))
            .Returns(expected);

        var result = CreateService().GetPersonById(10);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPartyByParticipantId_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["PartyID"] = 5 };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelPartyByParticipant",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 7)))
            .Returns(expected);

        var result = CreateService().GetPartyByParticipantId(7);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetProgramById_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["ProgramName"] = "Event" };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelProgram",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ProgramID"]! == 100)))
            .Returns(expected);

        var result = CreateService().GetProgramById(100);
        Assert.Equal(expected, result);
    }

    // Custom Fields — multi-row + void

    [Fact]
    public void GetCustomFieldValues_CallsExecuteStoredProcedureList()
    {
        var expected = new List<Dictionary<string, object?>> { new() { ["FieldName"] = "Dietary" } };
        _dbMock.Setup(d => d.ExecuteStoredProcedureList("spWRASelCustomFieldValues",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 42)))
            .Returns(expected);

        var result = CreateService().GetCustomFieldValues(42);
        Assert.Single(result);
        Assert.Equal("Dietary", result[0]["FieldName"]);
    }

    [Fact]
    public void SaveCustomFieldValue_CallsExecuteNonQuery()
    {
        CreateService().SaveCustomFieldValue(1, 2, "value", 3);

        _dbMock.Verify(d => d.ExecuteNonQuery("spWRAInsUpdCustomFieldValue",
            It.Is<Dictionary<string, object?>>(p =>
                (int)p["@ParticipantID"]! == 1 &&
                (int)p["@CustomFieldID"]! == 2 &&
                (string)p["@Value"]! == "value" &&
                (int)p["@PossibleValueID"]! == 3)),
            Times.Once);
    }

    // Accommodations

    [Fact]
    public void GetAccommodationDetails_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["HotelName"] = "Hilton" };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelAccommodation",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 5)))
            .Returns(expected);

        var result = CreateService().GetAccommodationDetails(5);
        Assert.Equal("Hilton", result!["HotelName"]);
    }

    [Fact]
    public void GetAccommodationList_CallsExecuteStoredProcedureList()
    {
        var expected = new List<Dictionary<string, object?>> { new() { ["Type"] = "Hotel" } };
        _dbMock.Setup(d => d.ExecuteStoredProcedureList("spWRASelAccommodationList",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 5)))
            .Returns(expected);

        var result = CreateService().GetAccommodationList(5);
        Assert.Single(result);
    }

    [Fact]
    public void SaveAccommodationRecord_CallsExecuteNonQuery_WithDataDict()
    {
        var data = new Dictionary<string, object?> { ["@HotelName"] = "Marriott" };
        CreateService().SaveAccommodationRecord(5, data);

        _dbMock.Verify(d => d.ExecuteNonQuery("spWRAInsUpdAccommodation",
            It.Is<Dictionary<string, object?>>(p =>
                (int)p["@ParticipantID"]! == 5 &&
                (string)p["@HotelName"]! == "Marriott")),
            Times.Once);
    }

    [Fact]
    public void DeleteAccommodationRecord_CallsExecuteNonQuery()
    {
        CreateService().DeleteAccommodationRecord(5, "Hotel");

        _dbMock.Verify(d => d.ExecuteNonQuery("spWRADelAccommodation",
            It.Is<Dictionary<string, object?>>(p =>
                (int)p["@ParticipantID"]! == 5 &&
                (string)p["@RecordType"]! == "Hotel")),
            Times.Once);
    }

    // Email Templates — string return

    [Fact]
    public void GetEmailTemplate_ExtractsFirstColumnAsString()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelEmailTemplate",
            It.Is<Dictionary<string, object?>>(p =>
                (string)p["@TemplateName"]! == "RegConfirm" &&
                (int)p["@ProgramID"]! == 1)))
            .Returns(new Dictionary<string, object?> { ["Body"] = "<html>Hello</html>" });

        var result = CreateService().GetEmailTemplate("RegConfirm", 1);
        Assert.Equal("<html>Hello</html>", result);
    }

    [Fact]
    public void GetEmailBody_ExtractsFirstColumnAsString()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelEmailBody",
            It.Is<Dictionary<string, object?>>(p =>
                (string)p["@TemplateKey"]! == "LogonCreds" &&
                (int)p["@ProgramID"]! == 2)))
            .Returns(new Dictionary<string, object?> { ["Content"] = "Your login is..." });

        var result = CreateService().GetEmailBody("LogonCreds", 2);
        Assert.Equal("Your login is...", result);
    }

    [Fact]
    public void GetEmailTemplate_ReturnsNull_WhenNoResult()
    {
        _dbMock.Setup(d => d.ExecuteStoredProcedure(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>()))
            .Returns((Dictionary<string, object?>?)null);

        var result = CreateService().GetEmailTemplate("Missing", 1);
        Assert.Null(result);
    }

    // Contact Numbers — multi-row

    [Fact]
    public void GetContactNumbers_CallsExecuteStoredProcedureList()
    {
        var expected = new List<Dictionary<string, object?>> { new() { ["Phone"] = "555-1234" } };
        _dbMock.Setup(d => d.ExecuteStoredProcedureList("spWRASelContactNumbers",
            It.Is<Dictionary<string, object?>>(p => (int)p["@PersonID"]! == 10)))
            .Returns(expected);

        var result = CreateService().GetContactNumbers(10);
        Assert.Single(result);
    }

    // Page Configuration

    [Fact]
    public void GetPageConfiguration_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["RequiresAuth"] = true };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelPageConfiguration",
            It.Is<Dictionary<string, object?>>(p =>
                (string)p["@PageName"]! == "profile" &&
                (string)p["@ProgramNumber"]! == "ABC")))
            .Returns(expected);

        var result = CreateService().GetPageConfiguration("profile", "ABC");
        Assert.Equal(true, result!["RequiresAuth"]);
    }

    // Transportation

    [Fact]
    public void GetTransportationDetails_CallsCorrectSP()
    {
        var expected = new Dictionary<string, object?> { ["Type"] = "Air" };
        _dbMock.Setup(d => d.ExecuteStoredProcedure("spWRASelTransportation",
            It.Is<Dictionary<string, object?>>(p => (int)p["@ParticipantID"]! == 3)))
            .Returns(expected);

        var result = CreateService().GetTransportationDetails(3);
        Assert.Equal("Air", result!["Type"]);
    }
}
