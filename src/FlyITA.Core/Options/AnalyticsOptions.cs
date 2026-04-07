namespace FlyITA.Core.Options;

public class AnalyticsOptions
{
    public const string SectionName = "Analytics";
    public bool Enabled { get; set; } = false;
    public string MeasurementId { get; set; } = string.Empty;
}
