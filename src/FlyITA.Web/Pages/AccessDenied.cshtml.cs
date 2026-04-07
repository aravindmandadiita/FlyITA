using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlyITA.Web.Pages;

public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
        HttpContext.Session.Clear();
        if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Session"))
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Session");
        }
    }
}
