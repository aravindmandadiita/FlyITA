using System.Text;

namespace FlyITA.Core.Services;

public static class EmailTemplateEngine
{
    public static string ReplaceParticipantTokens(string body, Dictionary<string, object?> participant)
    {
        body = ReplaceToken(body, "<<LegalFirstName>>", participant, "LegalFirstName");
        body = ReplaceToken(body, "<<LegalMiddleName>>", participant, "LegalMiddleName");
        body = ReplaceToken(body, "<<LegalLastName>>", participant, "LegalLastName");
        body = ReplaceToken(body, "<<DateOfBirth>>", participant, "DateOfBirth");
        body = ReplaceToken(body, "<<Gender>>", participant, "Gender");
        body = ReplaceToken(body, "<<EmailAddress>>", participant, "EmailAddress");
        body = ReplaceToken(body, "<<Prefix>>", participant, "Prefix");
        body = ReplaceToken(body, "<<Suffix>>", participant, "Suffix");

        body = body.Replace("[PARTICIPANT_NAME]",
            $"{GetValue(participant, "LegalFirstName")} {GetValue(participant, "LegalLastName")}".Trim());
        body = ReplaceToken(body, "[PARTICIPANT_EMAIL]", participant, "EmailAddress");
        body = ReplaceToken(body, "[PARTICIPANT_USERNAME]", participant, "UserName");
        body = ReplaceToken(body, "[PARTICIPANT_PASSWORD]", participant, "Password");
        body = ReplaceToken(body, "[FIRSTNAME]", participant, "LegalFirstName");
        body = ReplaceToken(body, "[LASTNAME]", participant, "LegalLastName");

        return body;
    }

    public static string ReplaceProgramTokens(string body, Dictionary<string, object?> program)
    {
        body = ReplaceToken(body, "[PROGRAM_NAME]", program, "ProgramName");
        body = ReplaceToken(body, "[PROGRAM_800NBR]", program, "ProgramTollFreeNbr");
        body = ReplaceToken(body, "[PROGRAM_HQNAME]", program, "TravelHeadquartersName");
        body = ReplaceToken(body, "[PROGRAM_URL]", program, "WebRegistrationSiteUrl");
        body = ReplaceToken(body, "[PROGRAM_EMAIL]", program, "FromEmailAddress");

        return body;
    }

    public static string ReplaceContactTokens(string body, List<Dictionary<string, object?>> contacts)
    {
        string business = "", fax = "", mobile = "", home = "";

        foreach (var contact in contacts)
        {
            var type = GetValue(contact, "ContactType").ToLowerInvariant();
            var number = GetValue(contact, "ContactNumber");

            switch (type)
            {
                case "business": business = number; break;
                case "fax": fax = number; break;
                case "mobile": mobile = number; break;
                case "home": home = number; break;
            }
        }

        body = body.Replace("<<BusinessPhone>>", business);
        body = body.Replace("<<BusinessFax>>", fax);
        body = body.Replace("<<MobilePhone>>", mobile);
        body = body.Replace("<<HomePhone>>", home);

        return body;
    }

    public static string ReplaceTransportationTokens(string body, Dictionary<string, object?> transportation)
    {
        body = ReplaceToken(body, "<<TransportationType>>", transportation, "TransportationType");
        body = ReplaceToken(body, "<<TravelDates>>", transportation, "TravelDates");
        body = ReplaceToken(body, "<<PrefHomeDepartureTime>>", transportation, "PrefHomeDepartureTime");
        body = ReplaceToken(body, "<<PrefDestDepartureTime>>", transportation, "PrefDestDepartureTime");
        body = ReplaceToken(body, "<<PrefAirline>>", transportation, "PrefAirline");
        body = ReplaceToken(body, "<<PrefHomeDepartureCity>>", transportation, "PrefHomeDepartureCity");
        body = ReplaceToken(body, "<<DriveTime>>", transportation, "DriveTime");
        body = ReplaceToken(body, "<<SeatPreference>>", transportation, "SeatPreference");
        body = ReplaceToken(body, "<<AirRemarks>>", transportation, "AirRemarks");
        body = ReplaceToken(body, "<<FrequentFlyerNumber>>", transportation, "FrequentFlyerNumber");
        body = ReplaceToken(body, "<<CheckInDate>>", transportation, "CheckInDate");
        body = ReplaceToken(body, "<<CheckOutDate>>", transportation, "CheckOutDate");
        body = ReplaceToken(body, "<<Wheelchair>>", transportation, "Wheelchair");
        body = ReplaceToken(body, "<<SpecialMeal>>", transportation, "SpecialMeal");

        return body;
    }

    public static string ReplaceCustomFieldTokens(string body, List<Dictionary<string, object?>> customFields)
    {
        if (!body.Contains("<<CustomField."))
            return body;

        foreach (var cf in customFields)
        {
            var name = GetValue(cf, "Name");
            var value = GetValue(cf, "Value");
            if (!string.IsNullOrEmpty(name))
                body = body.Replace($"<<CustomField.{name}>>", value);
        }

        return body;
    }

    public static string ReplaceFormFieldTokens(string body, Dictionary<string, string> formValues)
    {
        foreach (var (key, value) in formValues)
        {
            body = body.Replace($"[{key}]", value ?? "");
        }
        return body;
    }

    public static string ProcessRepeatingBlock(string body, string startTag, string endTag, List<Dictionary<string, string>> items)
    {
        int startsAt = body.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
        int endsAt = body.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);

        if (startsAt < 0 || endsAt < 0 || endsAt <= startsAt)
            return body;

        string template = body.Substring(startsAt + startTag.Length, endsAt - startsAt - startTag.Length);

        // Trim trailing newlines from template (matching legacy behavior)
        while (template.EndsWith("\r\n"))
            template = template[..^2];

        var expanded = new StringBuilder();
        foreach (var item in items)
        {
            string block = template;
            foreach (var (token, value) in item)
            {
                block = block.Replace(token, value ?? "");
            }
            expanded.Append(block);
        }

        // Replace the entire block (markers + template) with expanded content
        string fullBlock = body[startsAt..(endsAt + endTag.Length)];
        body = body.Replace(fullBlock, expanded.ToString());

        return body;
    }

    private static string ReplaceToken(string body, string placeholder, Dictionary<string, object?> data, string key)
    {
        return body.Replace(placeholder, GetValue(data, key));
    }

    private static string GetValue(Dictionary<string, object?> data, string key)
    {
        return data.TryGetValue(key, out var val) ? val?.ToString() ?? "" : "";
    }
}
