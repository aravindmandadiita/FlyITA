namespace FlyITA.Core.Options;

public class AnalyticsOptions
{
    public const string SectionName = "Analytics";
    public bool Enabled { get; set; }
    public string MeasurementId { get; set; } = "";
}
