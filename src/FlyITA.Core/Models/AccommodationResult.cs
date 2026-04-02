namespace FlyITA.Core.Models;

public class AccommodationResult
{
    public DateTime? GroupHotelDateBegin { get; set; }
    public DateTime? GroupHotelDateEnd { get; set; }
    public int PrimaryHotelRoomBlockID { get; set; }
    public int CruiseCabinBlockID { get; set; }
    public DateTime? CruiseDateBegin { get; set; }
    public DateTime? CruiseDateEnd { get; set; }
    public bool IsMultiDestination { get; set; }
    public int NonGroupHotelCount { get; set; }
    public bool HasAccommodations { get; set; }
}
