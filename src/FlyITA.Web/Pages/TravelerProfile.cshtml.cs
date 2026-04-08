using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;
using FlyITA.Web.Options;

namespace FlyITA.Web.Pages;

public class TravelerProfileModel : PageModel
{
    private readonly IPCentralDataAccess _dataAccess;
    private readonly ICaptchaService _captchaService;
    private readonly IEmailService _emailService;
    private readonly EmailOptions _emailOptions;

    public TravelerProfileModel(
        IPCentralDataAccess dataAccess,
        ICaptchaService captchaService,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions)
    {
        _dataAccess = dataAccess;
        _captchaService = captchaService;
        _emailService = emailService;
        _emailOptions = emailOptions.Value;
    }

    // Person data loaded on GET
    public Dictionary<string, object?>? Person { get; set; }
    public List<Dictionary<string, object?>> ContactNumbers { get; set; } = new();

    // Form fields bound on POST
    [BindProperty, Required] public string FirstName { get; set; } = "";
    [BindProperty, Required] public string LastName { get; set; } = "";
    [BindProperty, Required, EmailAddress] public string Email { get; set; } = "";
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

    public async Task<IActionResult> OnGetAsync(int? personId)
    {
        if (personId == null)
            return Page();

        await LoadPersonDataAsync(personId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? personId)
    {
        if (!ModelState.IsValid)
        {
            if (personId.HasValue) await LoadPersonDataAsync(personId.Value);
            return Page();
        }

        // Validate captcha
        if (!string.IsNullOrEmpty(CaptchaToken))
        {
            var captchaResult = await _captchaService.ValidateAsync(CaptchaToken);
            if (!captchaResult.IsValid)
            {
                ErrorMessage = "Captcha validation failed. Please try again.";
                if (personId.HasValue) await LoadPersonDataAsync(personId.Value);
                return Page();
            }
        }

        // Send traveler profile email (best-effort — don't fail the page if email fails)
        var emailData = new TravelerProfileEmailData
        {
            ToEmail = _emailOptions.ThirdPartyToEmail,
            FromEmail = !string.IsNullOrEmpty(_emailOptions.ThirdPartyFromEmail)
                ? _emailOptions.ThirdPartyFromEmail
                : _emailOptions.SmtpFrom,
            Subject = !string.IsNullOrEmpty(_emailOptions.ThirdPartySubject)
                ? _emailOptions.ThirdPartySubject
                : "Traveler Profile Submission",
            TravelerFirstName = FirstName,
            TravelerLastName = LastName,
            EmailAddress = Email,
            BusinessPhone = Phone,
            CompanyName = Company,
            FrequentFlyers = FrequentFlyers
                .Where(f => !string.IsNullOrWhiteSpace(f.Airline))
                .Select(f => new FrequentFlyerData { Airline = f.Airline, Number = f.MemberNumber })
                .ToList(),
            HotelMemberships = HotelMemberships
                .Where(h => !string.IsNullOrWhiteSpace(h.HotelChain))
                .Select(h => new HotelMembershipData { HotelChain = h.HotelChain, MemberNumber = h.MemberNumber })
                .ToList(),
            RentalCarMemberships = !string.IsNullOrWhiteSpace(RentalCarCompany)
                ? new List<RentalCarMembershipData> { new() { Company = RentalCarCompany, MemberNumber = RentalCarMemberNumber } }
                : new List<RentalCarMembershipData>()
        };

        try
        {
            await _emailService.SendTravelerProfileFormEmailAsync(emailData);
        }
        catch
        {
            // Best-effort — email failure should not prevent profile save
        }

        SuccessMessage = "Traveler profile saved successfully.";
        if (personId.HasValue) await LoadPersonDataAsync(personId.Value);
        return Page();
    }

    private async Task LoadPersonDataAsync(int personId)
    {
        Person = await _dataAccess.GetPersonByIdAsync(personId);
        ContactNumbers = await _dataAccess.GetContactNumbersAsync(personId);

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
