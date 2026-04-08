using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Web.Options;

namespace FlyITA.Web.Pages;

public class VacationModel : PageModel
{
    private readonly ICaptchaService _captchaService;
    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;

    public VacationModel(ICaptchaService captchaService, IEmailService emailService, IOptions<EmailOptions> emailOptions)
    {
        _captchaService = captchaService;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
    }

    // Contact info
    [BindProperty, Required(ErrorMessage = "Name is required.")]
    public string NameOfPersonRequesting { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Email is required."), EmailAddress]
    public string GeneralAndPassengerEmail { get; set; } = "";

    [BindProperty]
    public string PhoneNumber { get; set; } = "";

    // Passengers (up to 6)
    [BindProperty]
    public List<PassengerEntry> Passengers { get; set; } = new();

    // Frequent flyer programs (up to 5)
    [BindProperty]
    public List<FrequentFlyerEntry> FrequentFlyers { get; set; } = new();

    // Trip info
    [BindProperty, Required(ErrorMessage = "Departure city is required.")]
    public string DepartureCity { get; set; } = "";

    [BindProperty]
    public string PreferredAirline { get; set; } = "";

    [BindProperty, Required(ErrorMessage = "Destinations interested in is required.")]
    public string DestinationsInterestedIn { get; set; } = "";

    [BindProperty]
    public string PreferredDatesOfTravel { get; set; } = "";

    [BindProperty]
    public string DestinationsNotInterestedIn { get; set; } = "";

    [BindProperty]
    public string VacationDetails { get; set; } = "";

    [BindProperty]
    public string ImportantAmenities { get; set; } = "";

    [BindProperty]
    public string RoomsNeeded { get; set; } = "1";

    // Captcha
    [BindProperty]
    public string CaptchaToken { get; set; } = "";

    public string? ErrorMessage { get; set; }

    public static readonly string[] AirlineOptions = new[]
    {
        "", "Air Canada", "Air France", "Alaska Airlines", "Alitalia", "American",
        "British Air", "Cathay Pacific", "Continental", "Delta", "Emirates Air",
        "Frontier Air", "Hawaiian Air", "Japan Air", "Jet Blue Airways",
        "KLM Royal Dutch", "Lufthansa", "Midwest Express", "Northwest", "Qantas",
        "Southwest", "Swiss Airlines", "United", "US Airways"
    };

    public void OnGet()
    {
        // Initialize with 1 passenger by default
        if (Passengers.Count == 0)
            Passengers.Add(new PassengerEntry());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Enforce server-side max limits
        if (Passengers.Count > 6)
            Passengers = Passengers.Take(6).ToList();
        if (FrequentFlyers.Count > 5)
            FrequentFlyers = FrequentFlyers.Take(5).ToList();

        // Validate at least one passenger has a first name
        var validPassengers = Passengers.Where(p => !string.IsNullOrWhiteSpace(p.FirstName)).ToList();
        if (validPassengers.Count == 0)
        {
            ModelState.AddModelError("", "At least one passenger with a first name is required.");
            return Page();
        }

        // Validate passenger required fields (use original index for UI alignment)
        for (int i = 0; i < Passengers.Count; i++)
        {
            var p = Passengers[i];
            if (string.IsNullOrWhiteSpace(p.FirstName))
                continue;
            if (string.IsNullOrWhiteSpace(p.LastName))
                ModelState.AddModelError("", $"Passenger {i + 1} last name is required for {p.FirstName}.");
            if (string.IsNullOrWhiteSpace(p.DateOfBirth))
                ModelState.AddModelError("", $"Date of birth is required for {p.FirstName}.");
            if (string.IsNullOrWhiteSpace(p.Gender))
                ModelState.AddModelError("", $"Gender is required for {p.FirstName}.");
        }

        if (!ModelState.IsValid)
            return Page();

        // Validate captcha
        if (!string.IsNullOrEmpty(CaptchaToken))
        {
            var captchaResult = await _captchaService.ValidateAsync(CaptchaToken);
            if (!captchaResult.IsValid)
            {
                ErrorMessage = "Invalid captcha. Please try again.";
                return Page();
            }
        }

        // Build and send email via template
        try
        {
            var data = new VacationEmailData
            {
                ToEmail = _emailOptions.VacationToEmail,
                FromEmail = _emailOptions.SmtpFrom,
                Subject = !string.IsNullOrWhiteSpace(_emailOptions.VacationSubject) ? _emailOptions.VacationSubject : "Vacation Travel Request",
                NameOfPersonRequesting = NameOfPersonRequesting,
                GeneralAndPassengerEmail = GeneralAndPassengerEmail,
                PhoneNumber = PhoneNumber,
                DepartureCity = DepartureCity,
                PreferredAirline = PreferredAirline,
                DestinationsInterestedIn = DestinationsInterestedIn,
                PreferredDatesOfTravel = PreferredDatesOfTravel,
                DestinationsNotInterestedIn = DestinationsNotInterestedIn,
                VacationDetails = VacationDetails,
                ImportantAmenities = ImportantAmenities,
                RoomsNeeded = RoomsNeeded,
                Passengers = validPassengers.Select(p => new PassengerData
                {
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    PassportNumber = p.PassportNumber,
                    PassportExpiration = p.PassportExpiration
                }).ToList(),
                FrequentFlyers = FrequentFlyers
                    .Where(f => !string.IsNullOrWhiteSpace(f.Airline))
                    .Select(f => new FrequentFlyerData { Airline = f.Airline, Number = f.Number })
                    .ToList()
            };

            var emailResult = await _emailService.SendVacationRequestEmailAsync(data);
            if (!emailResult.IsValid)
            {
                ModelState.AddModelError("", "We were unable to submit your request right now. Please try again later.");
                return Page();
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "We were unable to submit your request right now. Please try again later.");
            return Page();
        }

        return RedirectToPage("/ThankYou", new { page = "Vacation" });
    }

    public class PassengerEntry
    {
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string DateOfBirth { get; set; } = "";
        public string Gender { get; set; } = "";
        public string PassportNumber { get; set; } = "";
        public string PassportExpiration { get; set; } = "";
    }

    public class FrequentFlyerEntry
    {
        public string Airline { get; set; } = "";
        public string Number { get; set; } = "";
    }
}
