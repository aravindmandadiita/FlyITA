using FlyITA.Core.Enums;

namespace FlyITA.Core.Interfaces;

public interface IContextManager
{
    // Base
    string DateFormat { get; }
    string SessionID { get; }

    // Identity
    int PersonID { get; set; }
    int ParticipantID { get; set; }
    int PartyID { get; set; }
    int ProgramID { get; set; }
    int RealUserID { get; set; }
    int ReportID { get; set; }
    int CultureID { get; set; }

    // Guest
    int CurrentGuestParticipantID { get; set; }
    int GuestMainCount { get; set; }
    int GuestAdditionalCount { get; set; }
    int GuestNotParticipatingCount { get; set; }
    int AllowedNumberOfGuests { get; set; }

    // Auth
    string LoginType { get; set; }
    bool CMSUser { get; set; }
    int ActingAsUserID { get; set; }
    string SAMLSessionID { get; set; }
    string CFNameSeamlessLogin { get; set; }

    // Travel
    TransportationSection TransportationSection { get; set; }
    string TransportationType { get; set; }
    string TravelDateBegin { get; set; }
    string TravelDateEnd { get; set; }
    double TravelDateBeginOA { get; set; }
    double TravelDateEndOA { get; set; }
    bool OvernightInFlight { get; set; }
    string TravelExtension { get; set; }

    // Accommodation
    string GroupHotelDateBegin { get; set; }
    string GroupHotelDateEnd { get; set; }
    bool GroupHotelDateSet { get; set; }
    int NonGroupHotelCount { get; set; }

    // Cruise
    string CruiseDateBegin { get; set; }
    string CruiseDateEnd { get; set; }

    // Registration
    bool IsRegistered { get; set; }
    bool IsAttending { get; set; }
    string AttendeeType { get; set; }
    string PartyRegistrationStatus { get; set; }
    string RegistrationClosedType { get; set; }

    // Fees
    string FeeDisplayTravelType { get; set; }
    bool FeeDisplayRentalCar { get; set; }
    bool FeeDisplayExtending { get; set; }
    bool FeeDisplayExtendingGroup { get; set; }
    bool FeeDisplayAccomAssistance { get; set; }

    // Mobile / Device
    string MobileType { get; set; }
    bool ReturnFromMobileDevice { get; set; }

    // Self-Enrollment
    bool IsSelfEnroll { get; set; }
    bool PersonMatching { get; set; }

    // Payment
    string CardToken { get; set; }
    string CardWebCaptureRequestId { get; set; }

    // Debug
    string ToDebugString();
}
