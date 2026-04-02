using FlyITA.Core.Enums;
using FlyITA.Core.Interfaces;

namespace FlyITA.Web.Services;

public class ContextManager : IContextManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public ContextManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Base
    public string DateFormat => "MM/dd/yyyy";
    public string SessionID => _httpContextAccessor.HttpContext?.Session.Id ?? string.Empty;

    // Identity
    public int PersonID { get => Session.GetInt32("PersonID"); set => Session.SetInt32("PersonID", value); }
    public int ParticipantID { get => Session.GetInt32("ParticipantID"); set => Session.SetInt32("ParticipantID", value); }
    public int PartyID { get => Session.GetInt32("PartyID"); set => Session.SetInt32("PartyID", value); }
    public int ProgramID { get => Session.GetInt32("ProgramID"); set => Session.SetInt32("ProgramID", value); }
    public int RealUserID { get => Session.GetInt32("RealUserID"); set => Session.SetInt32("RealUserID", value); }
    public int ReportID { get => Session.GetInt32("ReportID"); set => Session.SetInt32("ReportID", value); }
    public int CultureID { get => Session.GetInt32("CultureID"); set => Session.SetInt32("CultureID", value); }

    // Guest
    public int CurrentGuestParticipantID { get => Session.GetInt32("CurrentGuestParticipantID"); set => Session.SetInt32("CurrentGuestParticipantID", value); }
    public int GuestMainCount { get => Session.GetInt32("GuestMainCount"); set => Session.SetInt32("GuestMainCount", value); }
    public int GuestAdditionalCount { get => Session.GetInt32("GuestAdditionalCount"); set => Session.SetInt32("GuestAdditionalCount", value); }
    public int GuestNotParticipatingCount { get => Session.GetInt32("GuestNotParticipatingCount"); set => Session.SetInt32("GuestNotParticipatingCount", value); }
    public int AllowedNumberOfGuests { get => Session.GetInt32("AllowedNumberOfGuests"); set => Session.SetInt32("AllowedNumberOfGuests", value); }

    // Auth
    public string LoginType { get => Session.GetString("LoginType") ?? string.Empty; set => Session.SetString("LoginType", value); }
    public bool CMSUser { get => Session.GetBoolean("CMSUser"); set => Session.SetBoolean("CMSUser", value); }
    public int ActingAsUserID { get => Session.GetInt32("ActingAsUserID"); set => Session.SetInt32("ActingAsUserID", value); }
    public string SAMLSessionID { get => Session.GetString("SAMLSessionID") ?? string.Empty; set => Session.SetString("SAMLSessionID", value); }
    public string CFNameSeamlessLogin { get => Session.GetString("CFNameSeamlessLogin") ?? string.Empty; set => Session.SetString("CFNameSeamlessLogin", value); }

    // Travel
    public TransportationSection TransportationSection
    {
        get => (TransportationSection)Session.GetInt32("TransportationSection");
        set => Session.SetInt32("TransportationSection", (int)value);
    }
    public string TransportationType { get => Session.GetString("TransportationType") ?? string.Empty; set => Session.SetString("TransportationType", value); }
    public string TravelDateBegin { get => Session.GetString("TravelDateBegin") ?? string.Empty; set => Session.SetString("TravelDateBegin", value); }
    public string TravelDateEnd { get => Session.GetString("TravelDateEnd") ?? string.Empty; set => Session.SetString("TravelDateEnd", value); }
    public double TravelDateBeginOA { get => Session.GetDouble("TravelDateBeginOA"); set => Session.SetDouble("TravelDateBeginOA", value); }
    public double TravelDateEndOA { get => Session.GetDouble("TravelDateEndOA"); set => Session.SetDouble("TravelDateEndOA", value); }
    public bool OvernightInFlight { get => Session.GetBoolean("OvernightInFlight"); set => Session.SetBoolean("OvernightInFlight", value); }
    public string TravelExtension { get => Session.GetString("TravelExtension") ?? string.Empty; set => Session.SetString("TravelExtension", value); }

    // Accommodation
    public string GroupHotelDateBegin { get => Session.GetString("GroupHotelDateBegin") ?? string.Empty; set => Session.SetString("GroupHotelDateBegin", value); }
    public string GroupHotelDateEnd { get => Session.GetString("GroupHotelDateEnd") ?? string.Empty; set => Session.SetString("GroupHotelDateEnd", value); }
    public bool GroupHotelDateSet { get => Session.GetBoolean("GroupHotelDateSet"); set => Session.SetBoolean("GroupHotelDateSet", value); }
    public int NonGroupHotelCount { get => Session.GetInt32("NonGroupHotelCount"); set => Session.SetInt32("NonGroupHotelCount", value); }

    // Cruise
    public string CruiseDateBegin { get => Session.GetString("CruiseDateBegin") ?? string.Empty; set => Session.SetString("CruiseDateBegin", value); }
    public string CruiseDateEnd { get => Session.GetString("CruiseDateEnd") ?? string.Empty; set => Session.SetString("CruiseDateEnd", value); }

    // Registration
    public bool IsRegistered { get => Session.GetBoolean("IsRegistered"); set => Session.SetBoolean("IsRegistered", value); }
    public bool IsAttending { get => Session.GetBoolean("IsAttending"); set => Session.SetBoolean("IsAttending", value); }
    public string AttendeeType { get => Session.GetString("AttendeeType") ?? string.Empty; set => Session.SetString("AttendeeType", value); }
    public string PartyRegistrationStatus { get => Session.GetString("PartyRegistrationStatus") ?? string.Empty; set => Session.SetString("PartyRegistrationStatus", value); }
    public string RegistrationClosedType { get => Session.GetString("RegistrationClosedType") ?? string.Empty; set => Session.SetString("RegistrationClosedType", value); }

    // Fees
    public string FeeDisplayTravelType { get => Session.GetString("FeeDisplayTravelType") ?? string.Empty; set => Session.SetString("FeeDisplayTravelType", value); }
    public bool FeeDisplayRentalCar { get => Session.GetBoolean("FeeDisplayRentalCar"); set => Session.SetBoolean("FeeDisplayRentalCar", value); }
    public bool FeeDisplayExtending { get => Session.GetBoolean("FeeDisplayExtending"); set => Session.SetBoolean("FeeDisplayExtending", value); }
    public bool FeeDisplayExtendingGroup { get => Session.GetBoolean("FeeDisplayExtendingGroup"); set => Session.SetBoolean("FeeDisplayExtendingGroup", value); }
    public bool FeeDisplayAccomAssistance { get => Session.GetBoolean("FeeDisplayAccomAssistance"); set => Session.SetBoolean("FeeDisplayAccomAssistance", value); }

    // Mobile
    public string MobileType { get => Session.GetString("MobileType") ?? string.Empty; set => Session.SetString("MobileType", value); }
    public bool ReturnFromMobileDevice { get => Session.GetBoolean("ReturnFromMobileDevice"); set => Session.SetBoolean("ReturnFromMobileDevice", value); }

    // Self-Enrollment
    public bool IsSelfEnroll { get => Session.GetBoolean("IsSelfEnroll"); set => Session.SetBoolean("IsSelfEnroll", value); }
    public bool PersonMatching { get => Session.GetBoolean("PersonMatching"); set => Session.SetBoolean("PersonMatching", value); }

    // Payment
    public string CardToken { get => Session.GetString("CardToken") ?? string.Empty; set => Session.SetString("CardToken", value); }
    public string CardWebCaptureRequestId { get => Session.GetString("CardWebCaptureRequestId") ?? string.Empty; set => Session.SetString("CardWebCaptureRequestId", value); }

    public string ToDebugString()
    {
        return $"PersonID={PersonID}, ParticipantID={ParticipantID}, ProgramID={ProgramID}, " +
               $"IsRegistered={IsRegistered}, TransportationType={TransportationType}";
    }
}
