using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface IAccommodationsService
{
    Task<string> GetNextPageAsync(ValidationResult result);
    Task SetTransportationDatesAsync(ValidationResult result);
    Task SetCruiseDatesAsync();
    void SetGroupHotelDates(AccommodationResult result);
    Task SaveOverNightInFlightAsync();
    Task RemoveOverNightInFlightAsync();
    Task<AccommodationResult> GetAccommodationDetailsAsync(ValidationResult result);
    bool IsMultiDestination(AccommodationResult result);
    Task SetNonGroupHotelCountAsync();
    Task<int> GetPrimaryHotelRoomBlockIDAsync();
}
