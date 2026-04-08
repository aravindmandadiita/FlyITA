using FlyITA.Core.Services;

namespace FlyITA.Core.Tests.Services;

public class EmailTemplateEngineTests
{
    [Fact]
    public void ReplaceParticipantTokens_ReplacesAllKnownTokens()
    {
        var participant = new Dictionary<string, object?>
        {
            ["LegalFirstName"] = "John",
            ["LegalMiddleName"] = "M",
            ["LegalLastName"] = "Doe",
            ["DateOfBirth"] = "01/15/1990",
            ["Gender"] = "Male",
            ["EmailAddress"] = "john@test.com",
            ["UserName"] = "jdoe",
            ["Password"] = "secret"
        };

        var body = "Hello <<LegalFirstName>> <<LegalMiddleName>> <<LegalLastName>>, DOB: <<DateOfBirth>>, " +
                   "Gender: <<Gender>>, Email: <<EmailAddress>>, " +
                   "Name: [PARTICIPANT_NAME], User: [PARTICIPANT_USERNAME], Pass: [PARTICIPANT_PASSWORD]";

        var result = EmailTemplateEngine.ReplaceParticipantTokens(body, participant);

        Assert.Contains("John", result);
        Assert.Contains("Doe", result);
        Assert.Contains("01/15/1990", result);
        Assert.Contains("Male", result);
        Assert.Contains("john@test.com", result);
        Assert.Contains("jdoe", result);
        Assert.Contains("secret", result);
        Assert.Contains("John Doe", result); // [PARTICIPANT_NAME]
        Assert.DoesNotContain("<<", result);
        Assert.DoesNotContain("[PARTICIPANT_", result);
    }

    [Fact]
    public void ReplaceParticipantTokens_MissingKeys_ReplacesWithEmpty()
    {
        var participant = new Dictionary<string, object?> { ["LegalFirstName"] = "Jane" };
        var body = "<<LegalFirstName>> <<LegalLastName>>";

        var result = EmailTemplateEngine.ReplaceParticipantTokens(body, participant);

        Assert.Equal("Jane ", result);
    }

    [Fact]
    public void ReplaceProgramTokens_ReplacesAllKnownTokens()
    {
        var program = new Dictionary<string, object?>
        {
            ["ProgramName"] = "Rewards 2026",
            ["ProgramTollFreeNbr"] = "800-555-1234",
            ["TravelHeadquartersName"] = "ITA Group",
            ["WebRegistrationSiteUrl"] = "https://register.example.com",
            ["FromEmailAddress"] = "noreply@example.com"
        };

        var body = "[PROGRAM_NAME] [PROGRAM_800NBR] [PROGRAM_HQNAME] [PROGRAM_URL] [PROGRAM_EMAIL]";

        var result = EmailTemplateEngine.ReplaceProgramTokens(body, program);

        Assert.Equal("Rewards 2026 800-555-1234 ITA Group https://register.example.com noreply@example.com", result);
    }

    [Fact]
    public void ReplaceFormFieldTokens_ReplacesMatchingKeys()
    {
        var fields = new Dictionary<string, string>
        {
            ["NameofPersonRequesting"] = "Alice",
            ["PhoneNumber"] = "555-1234"
        };

        var body = "Name: [NameofPersonRequesting], Phone: [PhoneNumber], City: [DepartureCity]";

        var result = EmailTemplateEngine.ReplaceFormFieldTokens(body, fields);

        Assert.Contains("Alice", result);
        Assert.Contains("555-1234", result);
        Assert.Contains("[DepartureCity]", result); // Not in fields, left as-is
    }

    [Fact]
    public void ReplaceCustomFieldTokens_ReplacesMatchingFields()
    {
        var fields = new List<Dictionary<string, object?>>
        {
            new() { ["Name"] = "Title", ["Value"] = "VP" },
            new() { ["Name"] = "Department", ["Value"] = "Sales" }
        };

        var body = "Title: <<CustomField.Title>>, Dept: <<CustomField.Department>>";

        var result = EmailTemplateEngine.ReplaceCustomFieldTokens(body, fields);

        Assert.Equal("Title: VP, Dept: Sales", result);
    }

    [Fact]
    public void ReplaceCustomFieldTokens_NoCustomFields_ReturnsUnchanged()
    {
        var body = "No custom fields here";
        var result = EmailTemplateEngine.ReplaceCustomFieldTokens(body, new List<Dictionary<string, object?>>());
        Assert.Equal("No custom fields here", result);
    }

    [Fact]
    public void ProcessRepeatingBlock_MultipleItems_ExpandsBlock()
    {
        var body = "Before<passengerinfoi>Name: <<pfirstname>> <<plastname>>\n</passengerinfoi>After";
        var items = new List<Dictionary<string, string>>
        {
            new() { ["<<pfirstname>>"] = "Alice", ["<<plastname>>"] = "Smith" },
            new() { ["<<pfirstname>>"] = "Bob", ["<<plastname>>"] = "Jones" }
        };

        var result = EmailTemplateEngine.ProcessRepeatingBlock(body, "<passengerinfoi>", "</passengerinfoi>", items);

        Assert.Contains("Alice", result);
        Assert.Contains("Bob", result);
        Assert.Contains("Smith", result);
        Assert.Contains("Jones", result);
        Assert.DoesNotContain("<passengerinfoi>", result);
        Assert.DoesNotContain("</passengerinfoi>", result);
    }

    [Fact]
    public void ProcessRepeatingBlock_ZeroItems_RemovesBlock()
    {
        var body = "Before<passengerinfoi>Name: <<pfirstname>>\n</passengerinfoi>After";
        var items = new List<Dictionary<string, string>>();

        var result = EmailTemplateEngine.ProcessRepeatingBlock(body, "<passengerinfoi>", "</passengerinfoi>", items);

        Assert.Equal("BeforeAfter", result);
        Assert.DoesNotContain("<<pfirstname>>", result);
    }

    [Fact]
    public void ProcessRepeatingBlock_NoMarkers_ReturnsSameBody()
    {
        var body = "No blocks here";
        var items = new List<Dictionary<string, string>> { new() { ["<<x>>"] = "y" } };

        var result = EmailTemplateEngine.ProcessRepeatingBlock(body, "<block>", "</block>", items);

        Assert.Equal("No blocks here", result);
    }

    [Fact]
    public void ReplaceContactTokens_MapsToCorrectPhoneTypes()
    {
        var contacts = new List<Dictionary<string, object?>>
        {
            new() { ["ContactType"] = "Business", ["ContactNumber"] = "555-0001" },
            new() { ["ContactType"] = "Fax", ["ContactNumber"] = "555-0002" },
            new() { ["ContactType"] = "Mobile", ["ContactNumber"] = "555-0003" },
            new() { ["ContactType"] = "Home", ["ContactNumber"] = "555-0004" }
        };

        var body = "Bus: <<BusinessPhone>>, Fax: <<BusinessFax>>, Mob: <<MobilePhone>>, Home: <<HomePhone>>";

        var result = EmailTemplateEngine.ReplaceContactTokens(body, contacts);

        Assert.Equal("Bus: 555-0001, Fax: 555-0002, Mob: 555-0003, Home: 555-0004", result);
    }

    [Fact]
    public void ReplaceTransportationTokens_ReplacesAllTravelFields()
    {
        var transport = new Dictionary<string, object?>
        {
            ["TransportationType"] = "Air",
            ["SeatPreference"] = "Window",
            ["CheckInDate"] = "03/15/2026",
            ["CheckOutDate"] = "03/20/2026"
        };

        var body = "Type: <<TransportationType>>, Seat: <<SeatPreference>>, In: <<CheckInDate>>, Out: <<CheckOutDate>>";

        var result = EmailTemplateEngine.ReplaceTransportationTokens(body, transport);

        Assert.Equal("Type: Air, Seat: Window, In: 03/15/2026, Out: 03/20/2026", result);
    }
}
