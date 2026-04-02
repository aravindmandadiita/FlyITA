using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface IAccommodationsService
{
    string GetNextPage(ValidationResult result);
    void SetTransportationDates(ValidationResult result);
    void SetCruiseDates();
    void SetGroupHotelDates(AccommodationResult result);
    void SaveOverNightInFlight();
    void RemoveOverNightInFlight();
    AccommodationResult GetAccommodationDetails(ValidationResult result);
    bool IsMultiDestination(AccommodationResult result);
    void SetNonGroupHotelCount();
    int GetPrimaryHotelRoomBlockID();
}
