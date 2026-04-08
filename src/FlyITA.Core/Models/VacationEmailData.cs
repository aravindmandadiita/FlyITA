namespace FlyITA.Core.Models;

public class VacationEmailData
{
    // Email routing (page fills from EmailOptions)
    public string ToEmail { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;

    // Form fields matching legacy [Token] patterns
    public string NameOfPersonRequesting { get; set; } = string.Empty;
    public string GeneralAndPassengerEmail { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string DepartureCity { get; set; } = string.Empty;
    public string PreferredAirline { get; set; } = string.Empty;
    public string DestinationsInterestedIn { get; set; } = string.Empty;
    public string PreferredDatesOfTravel { get; set; } = string.Empty;
    public string DestinationsNotInterestedIn { get; set; } = string.Empty;
    public string VacationDetails { get; set; } = string.Empty;
    public string ImportantAmenities { get; set; } = string.Empty;
    public string RoomsNeeded { get; set; } = string.Empty;

    // Repeating blocks
    public List<PassengerData> Passengers { get; set; } = new();
    public List<FrequentFlyerData> FrequentFlyers { get; set; } = new();
}

public class PassengerData
{
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string PassportExpiration { get; set; } = string.Empty;
}

public class FrequentFlyerData
{
    public string Airline { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
}
