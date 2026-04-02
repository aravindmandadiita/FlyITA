namespace FlyITA.Core.Options;

public class ProgramOptions
{
    public const string SectionName = "Program";

    public int ProgramID { get; set; }
    public string ITAProgNbr { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
}
