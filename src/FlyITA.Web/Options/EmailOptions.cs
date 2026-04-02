namespace FlyITA.Web.Options;

public class EmailOptions
{
    public const string SectionName = "Email";
    public string SmtpFrom { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string VacationToEmail { get; set; } = string.Empty;
    public string VacationSubject { get; set; } = string.Empty;
    public string RegistrationConfirmationTemplate { get; set; } = string.Empty;
    public string LogonCredentialsTemplate { get; set; } = string.Empty;
    public string SeamlessLogonCredentialsTemplate { get; set; } = string.Empty;
    public string ThirdPartyEmailTemplate { get; set; } = string.Empty;
    public string TravelerProfileTemplate { get; set; } = string.Empty;
    public string VacationTravelRequestTemplate { get; set; } = string.Empty;
    public string ThirdPartyFromEmail { get; set; } = string.Empty;
    public string ThirdPartyToEmail { get; set; } = string.Empty;
    public string ThirdPartySubject { get; set; } = string.Empty;
}
