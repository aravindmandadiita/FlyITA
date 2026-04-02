namespace FlyITA.Web.Options;

public class PageOptions
{
    public const string SectionName = "Pages";
    public Dictionary<string, PageSettings> Pages { get; set; } = new();
}

public class PageSettings
{
    public bool RequiresAuthentication { get; set; } = true;
    public bool EnforceNav { get; set; } = true;
    public bool DisplayNav { get; set; } = true;
    public bool DisplayRegNav { get; set; } = false;
    public int RegNavLevel { get; set; } = 0;
    public bool DisplaySslSeal { get; set; } = false;
}
