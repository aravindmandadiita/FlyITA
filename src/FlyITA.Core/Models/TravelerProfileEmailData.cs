namespace FlyITA.Core.Models;

public class TravelerProfileEmailData
{
    // Email routing (page fills from EmailOptions)
    public string ToEmail { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;

    // General Information
    public string TravelerFirstName { get; set; } = string.Empty;
    public string TravelerMiddleName { get; set; } = string.Empty;
    public string TravelerLastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string TravelerTitle { get; set; } = string.Empty;
    public string DeptCostCenter { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string BusinessPhone { get; set; } = string.Empty;
    public string BusinessFax { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string HomePhone { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;

    // Passport Information
    public string PassportName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string PassportIssueDate { get; set; } = string.Empty;
    public string PassportExpirationDate { get; set; } = string.Empty;
    public string PlaceOfIssue { get; set; } = string.Empty;

    // Traveler Arranger
    public string TravelerArrangerName { get; set; } = string.Empty;
    public string TravelerArrangerPhone { get; set; } = string.Empty;
    public string TravelerArrangerEmail { get; set; } = string.Empty;

    // Emergency Contact
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;

    // Travel Preferences
    public string PreferredDepartureAirport { get; set; } = string.Empty;
    public string PreferredCarrier { get; set; } = string.Empty;
    public string OtherPreferredCarrier { get; set; } = string.Empty;
    public string SeatingPreference { get; set; } = string.Empty;
    public string SpecialMealRequirements { get; set; } = string.Empty;

    // Hotel Preferences
    public string SmokingPreference { get; set; } = string.Empty;
    public string BedPreference { get; set; } = string.Empty;
    public string SpecialRequirements { get; set; } = string.Empty;
    public string OtherHotelMembership { get; set; } = string.Empty;
    public string OtherHotelMembershipNumber { get; set; } = string.Empty;

    // Rental Car
    public string VehicleSize { get; set; } = string.Empty;

    // Repeating blocks
    public List<FrequentFlyerData> FrequentFlyers { get; set; } = new();
    public List<HotelMembershipData> HotelMemberships { get; set; } = new();
    public List<RentalCarMembershipData> RentalCarMemberships { get; set; } = new();
}

public class HotelMembershipData
{
    public string HotelChain { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
}

public class RentalCarMembershipData
{
    public string Company { get; set; } = string.Empty;
    public string MemberNumber { get; set; } = string.Empty;
}
