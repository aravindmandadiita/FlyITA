namespace FlyITA.Core.Options;

public class DisplayOptions
{
    public const string SectionName = "Display";

    public string DateFormat { get; set; } = "MM/dd/yyyy";
    public string TimeFormat { get; set; } = "hh:mm tt";
    public bool ScrollToWindowTop { get; set; } = true;
    public bool DisplaySSLSeal { get; set; } = false;
    public bool DisableForwardScript { get; set; } = true;
}
