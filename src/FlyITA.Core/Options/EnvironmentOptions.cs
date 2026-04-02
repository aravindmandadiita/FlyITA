namespace FlyITA.Core.Options;

public class EnvironmentOptions
{
    public const string SectionName = "Environment";

    public string Role { get; set; } = "CDT";
    public bool IsClientFacing { get; set; } = false;
    public string ConnectionStringName { get; set; } = "Default";
}
