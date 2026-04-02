namespace FlyITA.Web.Options;

public class EmailOptions
{
    public const string SectionName = "Email";
    public string SmtpFrom { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string VacationToEmail { get; set; } = string.Empty;
    public string VacationSubject { get; set; } = string.Empty;
}
