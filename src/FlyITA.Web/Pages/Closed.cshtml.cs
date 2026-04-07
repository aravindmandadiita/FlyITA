using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlyITA.Web.Pages;

public class ClosedModel : PageModel
{
    public string ClosedMessage { get; set; } = "Registration is currently closed. Please check back later or contact your program administrator.";

    public void OnGet()
    {
    }
}
