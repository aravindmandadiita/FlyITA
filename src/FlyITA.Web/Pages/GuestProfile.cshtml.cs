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
    [BindProperty] public string FirstName { get; set; } = "";
    [BindProperty] public string LastName { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
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

    public IActionResult OnGet(int? participantId)
    {
        if (participantId == null)
            return Page();

        LoadParticipantData(participantId.Value);
        return Page();
    }

    public IActionResult OnPost(int participantId)
    {
        if (!ModelState.IsValid)
        {
            LoadParticipantData(participantId);
            return Page();
        }

        // Save custom field values
        foreach (var field in CustomFields)
        {
            var fieldId = field.TryGetValue("CustomFieldID", out var fid) ? Convert.ToInt32(fid) : 0;
            var formValue = Request.Form[$"custom_{fieldId}"].ToString();
            if (fieldId > 0)
            {
                _dataAccess.SaveCustomFieldValue(participantId, fieldId, formValue, 0);
            }
        }

        SuccessMessage = "Profile saved successfully.";
        LoadParticipantData(participantId);
        return Page();
    }

    private void LoadParticipantData(int participantId)
    {
        Participant = _dataAccess.GetParticipantById(participantId);
        CustomFields = _dataAccess.GetCustomFieldValues(participantId);

        if (Participant != null)
        {
            FirstName = Participant.TryGetValue("FirstName", out var fn) ? fn?.ToString() ?? "" : "";
            LastName = Participant.TryGetValue("LastName", out var ln) ? ln?.ToString() ?? "" : "";
            Email = Participant.TryGetValue("Email", out var em) ? em?.ToString() ?? "" : "";
        }
    }
}
