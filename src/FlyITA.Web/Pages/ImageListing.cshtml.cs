using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlyITA.Web.Pages;

public class ImageListingModel : PageModel
{
    public IActionResult OnGet()
    {
        return new JsonResult(Array.Empty<object>());
    }
}
