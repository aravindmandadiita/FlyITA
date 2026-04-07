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

    public EmailService(IContextManager context, IPCentralDataAccess dataAccess, IConfiguration configuration, ISmtpClient smtpClient)
    {
        _context = context;
        _dataAccess = dataAccess;
        _configuration = configuration;
        _smtpClient = smtpClient;
    }

    public async Task<ValidationResult> SendRegistrationConfirmationAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null)
        {
            result.AddError("Participant not found.");
            return result;
        }

        var templateKey = _configuration["Email:RegistrationConfirmationTemplate"] ?? "RegistrationConfirmation";
        var body = await _dataAccess.GetEmailTemplateAsync(templateKey, _context.ProgramID);
        if (string.IsNullOrEmpty(body))
        {
            result.AddError("Email template not found.");
            return result;
        }

        body = await ReplacePlaceholdersAsync(body, participant);
        var subject = _configuration["Email:Subject"] ?? "Registration Confirmation";
        var to = GetParticipantEmail(participant);

        if (string.IsNullOrEmpty(to))
        {
            result.AddError("Participant email address not found.");
            return result;
        }

        await SendEmailAsync(to, subject, body, result);
        return result;
    }

    public async Task<ValidationResult> SendLogonCredentialsAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null)
        {
            result.AddError("Participant not found.");
            return result;
        }

        var templateKey = _configuration["Email:LogonCredentialsTemplate"] ?? "LogonCredentials";
        var body = await _dataAccess.GetEmailTemplateAsync(templateKey, _context.ProgramID);
        if (string.IsNullOrEmpty(body))
        {
            result.AddError("Email template not found.");
            return result;
        }

        body = await ReplacePlaceholdersAsync(body, participant);
        var to = GetParticipantEmail(participant);

        if (string.IsNullOrEmpty(to))
        {
            result.AddError("Participant email address not found.");
            return result;
        }

        await SendEmailAsync(to, "Your Login Credentials", body, result);
        return result;
    }

    public async Task<string> PreviewLogonCredentialsAsync(int participantId)
    {
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null) return "<p>Participant not found.</p>";

        var templateKey = _configuration["Email:LogonCredentialsTemplate"] ?? "LogonCredentials";
        var body = await _dataAccess.GetEmailTemplateAsync(templateKey, _context.ProgramID);
        if (string.IsNullOrEmpty(body)) return "<p>Template not found.</p>";

        return await ReplacePlaceholdersAsync(body, participant);
    }

    public async Task<ValidationResult> SendForgotPasswordCredentialsAsync(int participantId, string? seamlessLoginUrl = null)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null)
        {
            result.AddError("Participant not found.");
            return result;
        }

        var templateKey = seamlessLoginUrl != null
            ? (_configuration["Email:SeamlessLogonTemplate"] ?? "SeamlessLogon")
            : (_configuration["Email:ForgotPasswordTemplate"] ?? "ForgotPassword");

        var body = await _dataAccess.GetEmailTemplateAsync(templateKey, _context.ProgramID);
        if (string.IsNullOrEmpty(body))
        {
            result.AddError("Email template not found.");
            return result;
        }

        body = await ReplacePlaceholdersAsync(body, participant);
        if (seamlessLoginUrl != null)
            body = body.Replace("[SEAMLESS_LOGIN_URL]", seamlessLoginUrl);

        var to = GetParticipantEmail(participant);
        if (string.IsNullOrEmpty(to))
        {
            result.AddError("Participant email address not found.");
            return result;
        }

        await SendEmailAsync(to, "Password Reset", body, result);
        return result;
    }

    public async Task<ValidationResult> SendTravelerProfileEmailAsync(int participantId)
    {
        var result = new ValidationResult();
        var participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        if (participant == null)
        {
            result.AddError("Participant not found.");
            return result;
        }

        var templateKey = _configuration["Email:TravelerProfileTemplate"] ?? "TravelerProfile";
        var body = await _dataAccess.GetEmailTemplateAsync(templateKey, _context.ProgramID);
        if (string.IsNullOrEmpty(body))
        {
            result.AddError("Email template not found.");
            return result;
        }

        body = await ReplacePlaceholdersAsync(body, participant);
        var to = _configuration["Email:ToEmail"];

        if (string.IsNullOrEmpty(to))
        {
            result.AddError("Email:ToEmail configuration is not set.");
            return result;
        }

        await SendEmailAsync(to, "Traveler Profile Submission", body, result);
        return result;
    }

    public async Task<string> ReplacePlaceholdersAsync(string body, Dictionary<string, object?> participant)
    {
        body = ReplaceIfPresent(body, "<<LegalFirstName>>", participant, "LegalFirstName");
        body = ReplaceIfPresent(body, "<<LegalLastName>>", participant, "LegalLastName");
        body = ReplaceIfPresent(body, "<<LegalMiddleName>>", participant, "LegalMiddleName");
        body = ReplaceIfPresent(body, "<<EmailAddress>>", participant, "EmailAddress");
        body = ReplaceIfPresent(body, "<<Prefix>>", participant, "Prefix");
        body = ReplaceIfPresent(body, "<<Suffix>>", participant, "Suffix");
        body = ReplaceIfPresent(body, "<<TransportationType>>", participant, "TransportationType");
        body = ReplaceIfPresent(body, "<<CheckInDate>>", participant, "CheckInDate");
        body = ReplaceIfPresent(body, "<<CheckOutDate>>", participant, "CheckOutDate");

        body = body.Replace("[PARTICIPANT_NAME]",
            $"{GetValue(participant, "LegalFirstName")} {GetValue(participant, "LegalLastName")}");

        var programData = await _dataAccess.GetProgramByIdAsync(_context.ProgramID);
        if (programData != null)
        {
            body = ReplaceIfPresent(body, "[PROGRAM_URL]", programData, "ProgramURL");
            body = ReplaceIfPresent(body, "[PROGRAM_EMAIL]", programData, "ProgramEmail");
            body = ReplaceIfPresent(body, "[PROGRAM_PHONE]", programData, "ProgramPhone");
        }

        return body;
    }

    private static string ReplaceIfPresent(string body, string placeholder, Dictionary<string, object?> data, string key)
    {
        return body.Replace(placeholder, GetValue(data, key));
    }

    private static string GetValue(Dictionary<string, object?> data, string key)
    {
        return data.TryGetValue(key, out var val) ? val?.ToString() ?? "" : "";
    }

    private static string? GetParticipantEmail(Dictionary<string, object?> participant)
    {
        return participant.TryGetValue("EmailAddress", out var email) ? email?.ToString() : null;
    }

    private async Task SendEmailAsync(string to, string subject, string body, ValidationResult result)
    {
        try
        {
            var from = _configuration["Email:SmtpFrom"] ?? "noreply@itagroup.com";
            using var message = new MailMessage(from, to, subject, body) { IsBodyHtml = true };
            await _smtpClient.SendAsync(message);
        }
        catch (Exception ex)
        {
            result.AddError($"Failed to send email: {ex.Message}");
        }
    }
}
