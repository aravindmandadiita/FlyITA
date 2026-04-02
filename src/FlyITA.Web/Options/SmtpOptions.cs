namespace FlyITA.Web.Options;

public class SmtpOptions
{
    public const string SectionName = "Smtp";
    public string DeliveryMethod { get; set; } = "Network";
    public string? PickupDirectoryLocation { get; set; }
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
