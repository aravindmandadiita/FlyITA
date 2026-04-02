namespace FlyITA.Core.Options;

public class ExternalServicesOptions
{
    public const string SectionName = "ExternalServices";

    public string CardTokenServiceUrl { get; set; } = string.Empty;
    public string PerformanceCentralServiceUrl { get; set; } = string.Empty;
    public string CardProcessServiceUrl { get; set; } = string.Empty;
    public int ServiceTimeoutSeconds { get; set; } = 30;
}
