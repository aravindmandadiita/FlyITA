using Microsoft.AspNetCore.Mvc.RazorPages;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;

namespace FlyITA.Web.Pages;

public class LogoutModel : PageModel
{
    private readonly IContextManager _context;
    private readonly IPCentralDataAccess _dataAccess;

    public LogoutModel(IContextManager context, IPCentralDataAccess dataAccess)
    {
        _context = context;
        _dataAccess = dataAccess;
    }

    public string LogoutMessage { get; set; } = "You have been logged out successfully.";

    public async Task OnGetAsync(string? msg)
    {
        // Read program info before clearing session (need ProgramID while session is still active)
        string? programPhone = null;
        var programId = _context.ProgramID;
        if (programId > 0 && string.Equals(msg, "locked", StringComparison.OrdinalIgnoreCase))
        {
            var program = await _dataAccess.GetProgramByIdAsync(programId);
            if (program != null && program.TryGetValue("ContactPhone", out var phone))
                programPhone = phone?.ToString();
        }

        // Clear session
        HttpContext.Session.Clear();
        if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Session"))
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");
        }

        // Set message based on reason
        LogoutMessage = msg?.ToLowerInvariant() switch
        {
            "locked" => $"Your account has been locked. Please contact {programPhone ?? "Travel Headquarters"} for assistance.",
            "unknownpax" => "Your login credentials are not valid for this program.",
            "unknownprog" => "The requested program was not found.",
            "timeout" => "Your session has expired due to inactivity.",
            _ => "You have been logged out successfully."
        };
    }
}
