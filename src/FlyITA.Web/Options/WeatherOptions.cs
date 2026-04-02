namespace FlyITA.Web.Options;

public class WeatherOptions
{
    public const string SectionName = "Weather";
    public string ApiKey { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
}
