using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Options;

namespace FlyITA.Web.Pages;

public class ErrorModel : PageModel
{
    private readonly ErrorLoggingOptions _errorOptions;
    private readonly IEnvironmentService _environment;

    public ErrorModel(IOptions<ErrorLoggingOptions> errorOptions, IEnvironmentService environment)
    {
        _errorOptions = errorOptions.Value;
        _environment = environment;
    }

    public string ErrorTitle { get; set; } = "Error";
    public string ErrorMessage { get; set; } = "";
    public bool IsNotFound { get; set; }

    public void OnGet(string? code)
    {
        // Parse status code from query string or current response
        if (int.TryParse(code, out var parsedCode))
            Response.StatusCode = parsedCode;

        if (Response.StatusCode == 404 || code == "404")
        {
            IsNotFound = true;
            ErrorTitle = "Page Not Found";
            ErrorMessage = _errorOptions.NotFoundMessage;
            Response.StatusCode = 404;
        }
        else
        {
            ErrorTitle = "Error";
            ErrorMessage = _environment.IsClientFacing
                ? _errorOptions.ClientFacingErrorMessage
                : _errorOptions.InternalErrorMessage;
        }
    }
}
