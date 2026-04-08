using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface IEmailService
{
    Task<ValidationResult> SendRegistrationConfirmationAsync(int participantId);
    Task<ValidationResult> SendLogonCredentialsAsync(int participantId);
    Task<ValidationResult> SendForgotPasswordCredentialsAsync(int participantId, string? seamlessLoginUrl = null);
    Task<ValidationResult> SendTravelerProfileEmailAsync(int participantId);
    Task<string> PreviewLogonCredentialsAsync(int participantId);
    Task<ValidationResult> SendVacationRequestEmailAsync(VacationEmailData data);
    Task<ValidationResult> SendTravelerProfileFormEmailAsync(TravelerProfileEmailData data);
}
