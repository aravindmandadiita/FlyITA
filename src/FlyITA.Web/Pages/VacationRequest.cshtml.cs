using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FlyITA.Web.Pages;

public class VacationRequestModel : PageModel
{
    [BindProperty, Required] public string TravelerName { get; set; } = "";
    [BindProperty, Required, EmailAddress] public string TravelerEmail { get; set; } = "";
    [BindProperty] public string TravelerPhone { get; set; } = "";
    [BindProperty, Required] public string Destination { get; set; } = "";
    [BindProperty, Required] public string DepartureDate { get; set; } = "";
    [BindProperty, Required] public string ReturnDate { get; set; } = "";
    [BindProperty] public int NumberOfTravelers { get; set; } = 1;
    [BindProperty] public string FlightPreferences { get; set; } = "";
    [BindProperty] public string HotelPreferences { get; set; } = "";
    [BindProperty] public string CarRentalNeeded { get; set; } = "No";
    [BindProperty] public string AdditionalNotes { get; set; } = "";

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        // TODO: Send email notification with vacation travel request details
        SuccessMessage = "Your vacation travel request has been submitted. A travel advisor will contact you shortly.";
        return Page();
    }
}
