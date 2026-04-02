namespace FlyITA.Core.Options;

public class NavigationOptions
{
    public const string SectionName = "Navigation";

    public List<NavLinkConfig> Links { get; set; } = new();
    public List<NavLinkConfig> SplitNavLinks { get; set; } = new();
    public List<RegistrationStepConfig> RegistrationFlow { get; set; } = new();
    public List<RegistrationLevelConfig> RegistrationLevels { get; set; } = new();
    public SplitNavFormattingConfig SplitNavFormatting { get; set; } = new();
}

public class NavLinkConfig
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool ShowLeft { get; set; }
    public bool ShowFooter { get; set; }
    public bool ShowTop { get; set; }
    public string Registered { get; set; } = "always";
    public string? Role { get; set; }
    public string? Type { get; set; }
    public string? CssClass { get; set; }
}

public class RegistrationStepConfig
{
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class RegistrationLevelConfig
{
    public int Level { get; set; }
    public int RequiredLevel { get; set; }
}

public class SplitNavFormattingConfig
{
    public string LeftNavSeparator { get; set; } = string.Empty;
    public string TopNavSeparator { get; set; } = string.Empty;
    public string FooterSeparator { get; set; } = string.Empty;
}
