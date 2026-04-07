using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FlyITA.Core.Abstractions;

namespace FlyITA.Web.Pages;

public class GuestProfileModel : PageModel
{
    private readonly IPCentralDataAccess _dataAccess;

    public GuestProfileModel(IPCentralDataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    // Participant data loaded on GET
    public Dictionary<string, object?>? Participant { get; set; }
    public List<Dictionary<string, object?>> CustomFields { get; set; } = new();
    public Dictionary<string, object?>? PageConfig { get; set; }

    // Form fields bound on POST
    [BindProperty, Required] public string FirstName { get; set; } = "";
    [BindProperty, Required] public string LastName { get; set; } = "";
    [BindProperty, Required, EmailAddress] public string Email { get; set; } = "";
    [BindProperty] public string Phone { get; set; } = "";
    [BindProperty] public string Address1 { get; set; } = "";
    [BindProperty] public string Address2 { get; set; } = "";
    [BindProperty] public string City { get; set; } = "";
    [BindProperty] public string State { get; set; } = "";
    [BindProperty] public string ZipCode { get; set; } = "";
    [BindProperty] public string Country { get; set; } = "";
    [BindProperty] public string PassportNumber { get; set; } = "";
    [BindProperty] public string PassportCountry { get; set; } = "";
    [BindProperty] public string Citizenship { get; set; } = "";
    [BindProperty] public string DietaryRestrictions { get; set; } = "";
    [BindProperty] public string AssistanceNeeds { get; set; } = "";

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? participantId)
    {
        if (participantId == null)
            return Page();

        await LoadParticipantDataAsync(participantId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int participantId)
    {
        await LoadParticipantDataAsync(participantId);

        if (!ModelState.IsValid)
            return Page();

        // Save custom field values
        foreach (var field in CustomFields)
        {
            var fieldId = field.TryGetValue("CustomFieldID", out var fid) ? Convert.ToInt32(fid) : 0;
            var formValue = Request.Form[$"custom_{fieldId}"].ToString();
            if (fieldId > 0)
            {
                await _dataAccess.SaveCustomFieldValueAsync(participantId, fieldId, formValue, 0);
            }
        }

        SuccessMessage = "Profile saved successfully.";
        await LoadParticipantDataAsync(participantId);
        return Page();
    }

    private async Task LoadParticipantDataAsync(int participantId)
    {
        Participant = await _dataAccess.GetParticipantByIdAsync(participantId);
        CustomFields = await _dataAccess.GetCustomFieldValuesAsync(participantId);

        if (Participant != null)
        {
            FirstName = Participant.TryGetValue("FirstName", out var fn) ? fn?.ToString() ?? "" : "";
            LastName = Participant.TryGetValue("LastName", out var ln) ? ln?.ToString() ?? "" : "";
            Email = Participant.TryGetValue("Email", out var em) ? em?.ToString() ?? "" : "";
        }
    }
}
