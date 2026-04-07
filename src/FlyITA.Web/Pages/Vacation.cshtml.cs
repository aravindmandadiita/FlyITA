using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using FlyITA.Core.Interfaces;
using FlyITA.Web.Options;

namespace FlyITA.Web.Pages;

public class VacationModel : PageModel
{
    private readonly ICaptchaService _captchaService;
    private readonly ISmtpClient _smtpClient;
    private readonly EmailOptions _emailOptions;

    public VacationModel(ICaptchaService captchaService, ISmtpClient smtpClient, IOptions<EmailOptions> emailOptions)
    {
        _captchaService = captchaService;
        _smtpClient = smtpClient;
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

        // Build and send email
        try
        {
            await SendVacationRequestEmailAsync(validPassengers);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "We were unable to submit your request right now. Please try again later.");
            return Page();
        }

        return RedirectToPage("/ThankYou", new { page = "Vacation" });
    }

    private async Task SendVacationRequestEmailAsync(List<PassengerEntry> passengers)
    {
        var body = BuildEmailBody(passengers);
        var to = _emailOptions.VacationToEmail;
        var from = _emailOptions.SmtpFrom;
        var subject = _emailOptions.VacationSubject;

        if (!string.IsNullOrEmpty(to) && !string.IsNullOrEmpty(from))
        {
            using var message = new System.Net.Mail.MailMessage(from, to, !string.IsNullOrWhiteSpace(subject) ? subject : "Vacation Travel Request", body)
            {
                IsBodyHtml = true
            };
            await _smtpClient.SendAsync(message);
        }
    }

    private string BuildEmailBody(List<PassengerEntry> passengers)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<html><body>");
        sb.AppendLine("<h2>Vacation Travel Request</h2>");
        sb.AppendLine($"<p><strong>Requested by:</strong> {System.Net.WebUtility.HtmlEncode(NameOfPersonRequesting)}</p>");
        sb.AppendLine($"<p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(GeneralAndPassengerEmail)}</p>");
        sb.AppendLine($"<p><strong>Phone:</strong> {System.Net.WebUtility.HtmlEncode(PhoneNumber)}</p>");

        sb.AppendLine("<h3>Passengers</h3>");
        for (int i = 0; i < passengers.Count; i++)
        {
            var p = passengers[i];
            sb.AppendLine($"<p><strong>Passenger {i + 1}:</strong> {System.Net.WebUtility.HtmlEncode(p.FirstName)} {System.Net.WebUtility.HtmlEncode(p.MiddleName)} {System.Net.WebUtility.HtmlEncode(p.LastName)}<br>");
            sb.AppendLine($"DOB: {System.Net.WebUtility.HtmlEncode(p.DateOfBirth)} | Gender: {System.Net.WebUtility.HtmlEncode(p.Gender)}<br>");
            if (!string.IsNullOrWhiteSpace(p.PassportNumber))
                sb.AppendLine($"Passport: {System.Net.WebUtility.HtmlEncode(p.PassportNumber)} (Exp: {System.Net.WebUtility.HtmlEncode(p.PassportExpiration)})<br>");
            sb.AppendLine("</p>");
        }

        var activeFlyers = FrequentFlyers.Where(f => !string.IsNullOrWhiteSpace(f.Airline)).ToList();
        if (activeFlyers.Count > 0)
        {
            sb.AppendLine("<h3>Frequent Flyer Programs</h3>");
            foreach (var ff in activeFlyers)
                sb.AppendLine($"<p>{System.Net.WebUtility.HtmlEncode(ff.Airline)}: {System.Net.WebUtility.HtmlEncode(ff.Number)}</p>");
        }

        sb.AppendLine("<h3>Trip Information</h3>");
        sb.AppendLine($"<p><strong>Departure City:</strong> {System.Net.WebUtility.HtmlEncode(DepartureCity)}</p>");
        sb.AppendLine($"<p><strong>Preferred Airline:</strong> {System.Net.WebUtility.HtmlEncode(PreferredAirline)}</p>");
        sb.AppendLine($"<p><strong>Destinations:</strong> {System.Net.WebUtility.HtmlEncode(DestinationsInterestedIn)}</p>");
        sb.AppendLine($"<p><strong>Preferred Dates:</strong> {System.Net.WebUtility.HtmlEncode(PreferredDatesOfTravel)}</p>");
        if (!string.IsNullOrWhiteSpace(DestinationsNotInterestedIn))
            sb.AppendLine($"<p><strong>Not Interested In:</strong> {System.Net.WebUtility.HtmlEncode(DestinationsNotInterestedIn)}</p>");
        if (!string.IsNullOrWhiteSpace(VacationDetails))
            sb.AppendLine($"<p><strong>Details:</strong> {System.Net.WebUtility.HtmlEncode(VacationDetails)}</p>");
        if (!string.IsNullOrWhiteSpace(ImportantAmenities))
            sb.AppendLine($"<p><strong>Amenities:</strong> {System.Net.WebUtility.HtmlEncode(ImportantAmenities)}</p>");
        sb.AppendLine($"<p><strong>Rooms Needed:</strong> {System.Net.WebUtility.HtmlEncode(RoomsNeeded)}</p>");

        sb.AppendLine("</body></html>");
        return sb.ToString();
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
