namespace FlyITA.Web.Options;

public class ErrorLoggingOptions
{
    public const string SectionName = "ErrorLogging";
    public int SystemId { get; set; } = 926;
    public int MaxMessageChunkSize { get; set; } = 4090;
    public string ClientFacingErrorMessage { get; set; } = "The system is experiencing an unexpected error. Please try again later or contact Travel Headquarters.";
    public string InternalErrorMessage { get; set; } = "An internal error occurred. See error details below.";
    public string NotFoundMessage { get; set; } = "The page you're looking for does not exist on our server.";
}
