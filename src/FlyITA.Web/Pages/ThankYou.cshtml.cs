using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlyITA.Web.Pages;

public class ThankYouModel : PageModel
{
    public string HeaderText { get; set; } = "Thank You";
    public string MessageText { get; set; } = "Your registration has been completed successfully.";

    public void OnGet()
    {
        var page = Request.Query["page"].FirstOrDefault();
        if (string.Equals(page, "Vacation", StringComparison.OrdinalIgnoreCase))
        {
            HeaderText = "Vacation Travel Request";
            MessageText = "Your vacation travel request has been submitted successfully. A travel advisor will contact you shortly.";
        }
    }
}
