using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;

namespace FlyITA.Core.Services;

public class EmailService : IEmailService
{
    private readonly IContextManager _context;
    private readonly IPCentralDataAccess _dataAccess;
    private readonly IConfiguration _configuration;
    private readonly ISmtpClient _smtpClient;
    private readonly IEmailTemplateLoader _templateLoader;

    public EmailService(
        IContextManager context,
        IPCentralDataAccess dataAccess,
        IConfiguration configuration,
        ISmtpClient smtpClient,
        IEmailTemplateLoader templateLoader)
    {
        _context = context;
        _dataAccess = dataAccess;
        _configuration = configuration;
        _smtpClient = smtpClient;
        _templateLoader = templateLoader;
    }

    public async Task<ValidationResult> SendRegistrationConfirmationAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) { result.AddError("Participant not found."); return result; }

        var templateName = _configuration["Email:RegistrationConfirmationTemplate"] ?? "RegistrationConfirmation";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Email template not found."); return result; }

        body = await ReplaceStandardTokensAsync(body, participant);
        var subject = _configuration["Email:Subject"] ?? "Registration Confirmation";
        var to = GetParticipantEmail(participant);
        if (string.IsNullOrEmpty(to)) { result.AddError("Participant email address not found."); return result; }

        await SendEmailAsync(to, subject, body, result);
        return result;
    }

    public async Task<ValidationResult> SendLogonCredentialsAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) { result.AddError("Participant not found."); return result; }

        var templateName = _configuration["Email:LogonCredentialsTemplate"] ?? "LogonCredentials";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Email template not found."); return result; }

        body = await ReplaceStandardTokensAsync(body, participant);
        var to = GetParticipantEmail(participant);
        if (string.IsNullOrEmpty(to)) { result.AddError("Participant email address not found."); return result; }

        await SendEmailAsync(to, "Your Login Credentials", body, result);
        return result;
    }

    public async Task<string> PreviewLogonCredentialsAsync(int participantId)
    {
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) return "<p>Participant not found.</p>";

        var templateName = _configuration["Email:LogonCredentialsTemplate"] ?? "LogonCredentials";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) return "<p>Template not found.</p>";

        return await ReplaceStandardTokensAsync(body, participant);
    }

    public async Task<ValidationResult> SendForgotPasswordCredentialsAsync(int participantId, string? seamlessLoginUrl = null)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) { result.AddError("Participant not found."); return result; }

        var templateName = seamlessLoginUrl != null
            ? (_configuration["Email:SeamlessLogonCredentialsTemplate"] ?? "SeamlessLogon")
            : (_configuration["Email:ForgotPasswordTemplate"] ?? "ForgotPassword");

        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Email template not found."); return result; }

        body = await ReplaceStandardTokensAsync(body, participant);
        if (seamlessLoginUrl != null)
            body = body.Replace("[SEAMLESS_LOGIN_URL]", seamlessLoginUrl);

        var to = GetParticipantEmail(participant);
        if (string.IsNullOrEmpty(to)) { result.AddError("Participant email address not found."); return result; }

        await SendEmailAsync(to, "Password Reset", body, result);
        return result;
    }

    public async Task<ValidationResult> SendTravelerProfileEmailAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) { result.AddError("Participant not found."); return result; }

        var templateName = _configuration["Email:TravelerProfileTemplate"] ?? "TravelerProfile";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Email template not found."); return result; }

        body = await ReplaceStandardTokensAsync(body, participant);
        var to = _configuration["Email:ToEmail"];
        if (string.IsNullOrEmpty(to)) { result.AddError("Email:ToEmail configuration is not set."); return result; }

        await SendEmailAsync(to, "Traveler Profile Submission", body, result);
        return result;
    }

    public async Task<ValidationResult> SendVacationRequestEmailAsync(VacationEmailData data)
    {
        var result = new ValidationResult();

        var templateName = _configuration["Email:VacationTravelRequestTemplate"] ?? "VacationTravelRequest.html";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Vacation email template not found."); return result; }

        // Replace form field tokens
        var formFields = new Dictionary<string, string>
        {
            ["NameofPersonRequesting"] = data.NameOfPersonRequesting,
            ["GeneralandPassengerEmail"] = data.GeneralAndPassengerEmail,
            ["PhoneNumber"] = data.PhoneNumber,
            ["DepartureCity"] = data.DepartureCity,
            ["PreferredAirline"] = data.PreferredAirline,
            ["DestinationsInterestedIn"] = data.DestinationsInterestedIn,
            ["PreferredDatesofTravel"] = data.PreferredDatesOfTravel,
            ["DestinationsNotInterestedIn"] = data.DestinationsNotInterestedIn,
            ["VacationDetails"] = data.VacationDetails,
            ["ImportantAmenities"] = data.ImportantAmenities,
            ["RoomsNeeded"] = data.RoomsNeeded
        };
        body = EmailTemplateEngine.ReplaceFormFieldTokens(body, formFields);

        // Process passenger repeating block
        var passengers = data.Passengers.Select(p => new Dictionary<string, string>
        {
            ["<<pfirstname>>"] = p.FirstName,
            ["<<pmiddlename>>"] = p.MiddleName,
            ["<<plastname>>"] = p.LastName,
            ["<<pdob>>"] = p.DateOfBirth,
            ["<<pgender>>"] = p.Gender,
            ["<<ppassportnumber>>"] = p.PassportNumber,
            ["<<ppassportexp>>"] = p.PassportExpiration
        }).ToList();
        body = EmailTemplateEngine.ProcessRepeatingBlock(body, "<passengerinfoi>", "</passengerinfoi>", passengers);

        // Process frequent flyer repeating block
        var flyers = data.FrequentFlyers.Select(f => new Dictionary<string, string>
        {
            ["<<frequentflyernumber>>"] = f.Airline,
            ["<<frequentflyernumberid>>"] = f.Number
        }).ToList();
        body = EmailTemplateEngine.ProcessRepeatingBlock(body, "<frequentflyerti>", "</frequentflyerti>", flyers);

        var to = data.ToEmail;
        var from = data.FromEmail;
        var subject = data.Subject;

        if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(from))
        {
            result.AddError("Vacation email routing not configured.");
            return result;
        }

        await SendEmailAsync(to, from, subject, body, result);
        return result;
    }

    public async Task<ValidationResult> SendTravelerProfileFormEmailAsync(TravelerProfileEmailData data)
    {
        var result = new ValidationResult();

        var templateName = _configuration["Email:TravelerProfileTemplate"] ?? "TravelerProfiler.html";
        var body = await _templateLoader.LoadTemplateAsync(templateName);
        if (string.IsNullOrEmpty(body)) { result.AddError("Traveler profile email template not found."); return result; }

        // Replace form field tokens
        var formFields = new Dictionary<string, string>
        {
            ["TravelerFirstName"] = data.TravelerFirstName,
            ["TravelerMiddleName"] = data.TravelerMiddleName,
            ["TravelerLastName"] = data.TravelerLastName,
            ["CompanyName"] = data.CompanyName,
            ["TravelerTitle"] = data.TravelerTitle,
            ["DeptCostCenter"] = data.DeptCostCenter,
            ["EmailAddress"] = data.EmailAddress,
            ["BusinessPhone"] = data.BusinessPhone,
            ["BusinessFax"] = data.BusinessFax,
            ["MobilePhone"] = data.MobilePhone,
            ["HomePhone"] = data.HomePhone,
            ["BirthDate"] = data.BirthDate,
            ["Gender"] = data.Gender,
            ["PassportName"] = data.PassportName,
            ["PassportNumber"] = data.PassportNumber,
            ["PassportIssueDate"] = data.PassportIssueDate,
            ["PassportExpirationDate"] = data.PassportExpirationDate,
            ["PlaceofIssue"] = data.PlaceOfIssue,
            ["TravelerArrangerName"] = data.TravelerArrangerName,
            ["TravelerArrangerPhone"] = data.TravelerArrangerPhone,
            ["TravelerArrangerEmail"] = data.TravelerArrangerEmail,
            ["EmergencyContactName"] = data.EmergencyContactName,
            ["EmergencyContactRelationship"] = data.EmergencyContactRelationship,
            ["EmergencyContactPhone"] = data.EmergencyContactPhone,
            ["PreferredDepartureAirport"] = data.PreferredDepartureAirport,
            ["PreferredCarrier"] = data.PreferredCarrier,
            ["OtherPreferredCarrier"] = data.OtherPreferredCarrier,
            ["SeatingPreference"] = data.SeatingPreference,
            ["SpecialMealRequirements"] = data.SpecialMealRequirements,
            ["SmokingPreference"] = data.SmokingPreference,
            ["BedPreference"] = data.BedPreference,
            ["SpecialRequirements"] = data.SpecialRequirements,
            ["OtherHotelMembership"] = data.OtherHotelMembership,
            ["OtherHotelMembershipNumber"] = data.OtherHotelMembershipNumber,
            ["VehicleSize"] = data.VehicleSize
        };
        body = EmailTemplateEngine.ReplaceFormFieldTokens(body, formFields);

        // Process repeating blocks
        var flyers = data.FrequentFlyers.Select(f => new Dictionary<string, string>
        {
            ["<<frequentflyernumber>>"] = f.Airline,
            ["<<frequentflyernumberid>>"] = f.Number
        }).ToList();
        body = EmailTemplateEngine.ProcessRepeatingBlock(body, "<frequentflyerti>", "</frequentflyerti>", flyers);

        var hotels = data.HotelMemberships.Select(h => new Dictionary<string, string>
        {
            ["<<hotelclubmemberships>>"] = h.HotelChain,
            ["<<hotelclubmembershipsid>>"] = h.MemberNumber
        }).ToList();
        body = EmailTemplateEngine.ProcessRepeatingBlock(body, "<hotelclubmembershipsti>", "</hotelclubmembershipsti>", hotels);

        var cars = data.RentalCarMemberships.Select(c => new Dictionary<string, string>
        {
            ["<<rentalcarmemberships>>"] = c.Company,
            ["<<rentalcarmembershipsid>>"] = c.MemberNumber
        }).ToList();
        body = EmailTemplateEngine.ProcessRepeatingBlock(body, "<rentalcarmembershipsti>", "</rentalcarmembershipsti>", cars);

        var to = data.ToEmail;
        var from = data.FromEmail;
        var subject = data.Subject;

        if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(from))
        {
            result.AddError("Traveler profile email routing not configured.");
            return result;
        }

        await SendEmailAsync(to, from, subject, body, result);
        return result;
    }

    private async Task<string> ReplaceStandardTokensAsync(string body, Dictionary<string, object?> participant)
    {
        body = EmailTemplateEngine.ReplaceParticipantTokens(body, participant);

        var program = await _dataAccess.GetProgramByIdAsync(_context.ProgramID);
        if (program != null)
            body = EmailTemplateEngine.ReplaceProgramTokens(body, program);

        return body;
    }

    private static string? GetParticipantEmail(Dictionary<string, object?> participant)
    {
        return participant.TryGetValue("EmailAddress", out var email) ? email?.ToString() : null;
    }

    private async Task SendEmailAsync(string to, string subject, string body, ValidationResult result)
    {
        var from = _configuration["Email:SmtpFrom"] ?? "noreply@itagroup.com";
        await SendEmailAsync(to, from, subject, body, result);
    }

    private async Task SendEmailAsync(string to, string from, string subject, string body, ValidationResult result)
    {
        try
        {
            using var message = new MailMessage(from, to, subject, body) { IsBodyHtml = true };
            await _smtpClient.SendAsync(message);
        }
        catch (Exception ex)
        {
            result.AddError($"Failed to send email: {ex.Message}");
        }
    }
}
