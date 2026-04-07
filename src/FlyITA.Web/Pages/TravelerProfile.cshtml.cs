using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;

namespace FlyITA.Web.Pages;

public class TravelerProfileModel : PageModel
{
    private readonly IPCentralDataAccess _dataAccess;
    private readonly ICaptchaService _captchaService;

    public TravelerProfileModel(IPCentralDataAccess dataAccess, ICaptchaService captchaService)
    {
        _dataAccess = dataAccess;
        _captchaService = captchaService;
    }

    // Person data loaded on GET
    public Dictionary<string, object?>? Person { get; set; }
    public List<Dictionary<string, object?>> ContactNumbers { get; set; } = new();

    // Form fields bound on POST
    [BindProperty] public string FirstName { get; set; } = "";
    [BindProperty] public string LastName { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string Company { get; set; } = "";

    // Frequent flyer entries (repeater)
    [BindProperty] public List<FrequentFlyerEntry> FrequentFlyers { get; set; } = new();

    // Hotel memberships (repeater)
    [BindProperty] public List<HotelMembershipEntry> HotelMemberships { get; set; } = new();

    // Rental car preferences
    [BindProperty] public string RentalCarCompany { get; set; } = "";
    [BindProperty] public string RentalCarMemberNumber { get; set; } = "";

    // Captcha
    [BindProperty] public string CaptchaToken { get; set; } = "";

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet(int? personId)
    {
        if (personId == null)
            return Page();

        LoadPersonData(personId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? personId)
    {
        if (!ModelState.IsValid)
        {
            if (personId.HasValue) LoadPersonData(personId.Value);
            return Page();
        }

        // Validate captcha
        if (!string.IsNullOrEmpty(CaptchaToken))
        {
            var captchaResult = await _captchaService.ValidateAsync(CaptchaToken);
            if (!captchaResult.IsValid)
            {
                ErrorMessage = "Captcha validation failed. Please try again.";
                if (personId.HasValue) LoadPersonData(personId.Value);
                return Page();
            }
        }

        SuccessMessage = "Traveler profile saved successfully.";
        if (personId.HasValue) LoadPersonData(personId.Value);
        return Page();
    }

    private void LoadPersonData(int personId)
    {
        Person = _dataAccess.GetPersonById(personId);
        ContactNumbers = _dataAccess.GetContactNumbers(personId);

        if (Person != null)
        {
            FirstName = Person.TryGetValue("FirstName", out var fn) ? fn?.ToString() ?? "" : "";
            LastName = Person.TryGetValue("LastName", out var ln) ? ln?.ToString() ?? "" : "";
            Email = Person.TryGetValue("Email", out var em) ? em?.ToString() ?? "" : "";
        }
    }

    public class FrequentFlyerEntry
    {
        public string Airline { get; set; } = "";
        public string MemberNumber { get; set; } = "";
    }

    public class HotelMembershipEntry
    {
        public string HotelChain { get; set; } = "";
        public string MemberNumber { get; set; } = "";
    }
}
