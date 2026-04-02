using FlyITA.Core.Abstractions;
using FlyITA.Core.Interfaces;
using FlyITA.Core.Models;

namespace FlyITA.Core.Services;

public class AccommodationsService : IAccommodationsService
{
    private readonly IContextManager _context;
    private readonly IPCentralDataAccess _dataAccess;
    private readonly INavigationService _navigation;

    public AccommodationsService(IContextManager context, IPCentralDataAccess dataAccess, INavigationService navigation)
    {
        _context = context;
        _dataAccess = dataAccess;
        _navigation = navigation;
    }

    public string GetNextPage(ValidationResult result)
    {
        var accomResult = GetAccommodationDetails(result);

        if (accomResult.IsMultiDestination)
            return _navigation.GetNextPageByPage("accommodations", "multidestination");

        if (accomResult.CruiseCabinBlockID > 0)
            return _navigation.GetNextPageByPage("accommodations", "cruise");

        return _navigation.GetNextPageByPage("accommodations", "default");
    }

    public void SetTransportationDates(ValidationResult result)
    {
        var transport = _dataAccess.GetTransportationDetails(_context.ParticipantID);
        if (transport == null)
        {
            result.AddError("Unable to retrieve transportation details.");
            return;
        }

        if (transport.TryGetValue("DepartureDate", out var depDate) && depDate is DateTime dep)
            _context.TravelDateBegin = dep.ToString(_context.DateFormat);

        if (transport.TryGetValue("ReturnDate", out var retDate) && retDate is DateTime ret)
            _context.TravelDateEnd = ret.ToString(_context.DateFormat);
    }

    public void SetCruiseDates()
    {
        var accom = _dataAccess.GetAccommodationDetails(_context.ParticipantID);
        if (accom == null) return;

        if (accom.TryGetValue("CruiseDateBegin", out var begin) && begin is DateTime cruiseBegin)
            _context.CruiseDateBegin = cruiseBegin.ToString(_context.DateFormat);

        if (accom.TryGetValue("CruiseDateEnd", out var end) && end is DateTime cruiseEnd)
            _context.CruiseDateEnd = cruiseEnd.ToString(_context.DateFormat);
    }

    public void SetGroupHotelDates(AccommodationResult result)
    {
        if (result.GroupHotelDateBegin.HasValue)
            _context.GroupHotelDateBegin = result.GroupHotelDateBegin.Value.ToString(_context.DateFormat);

        if (result.GroupHotelDateEnd.HasValue)
            _context.GroupHotelDateEnd = result.GroupHotelDateEnd.Value.ToString(_context.DateFormat);

        _context.GroupHotelDateSet = true;
    }

    public void SaveOverNightInFlight()
    {
        _dataAccess.SaveAccommodationRecord(_context.ParticipantID, new Dictionary<string, object?>
        {
            ["RecordType"] = "OvernightInFlight",
            ["ParticipantID"] = _context.ParticipantID
        });
        _context.OvernightInFlight = true;
    }

    public void RemoveOverNightInFlight()
    {
        _dataAccess.DeleteAccommodationRecord(_context.ParticipantID, "OvernightInFlight");
        _context.OvernightInFlight = false;
    }

    public AccommodationResult GetAccommodationDetails(ValidationResult result)
    {
        var accomResult = new AccommodationResult();
        var data = _dataAccess.GetAccommodationDetails(_context.ParticipantID);

        if (data == null)
        {
            result.AddWarning("No accommodation details found.");
            return accomResult;
        }

        accomResult.HasAccommodations = true;

        if (data.TryGetValue("GroupHotelDateBegin", out var ghBegin) && ghBegin is DateTime ghb)
            accomResult.GroupHotelDateBegin = ghb;

        if (data.TryGetValue("GroupHotelDateEnd", out var ghEnd) && ghEnd is DateTime ghe)
            accomResult.GroupHotelDateEnd = ghe;

        if (data.TryGetValue("PrimaryHotelRoomBlockID", out var roomBlock) && roomBlock is int rb)
            accomResult.PrimaryHotelRoomBlockID = rb;

        if (data.TryGetValue("CruiseCabinBlockID", out var cruise) && cruise is int cc)
            accomResult.CruiseCabinBlockID = cc;

        if (data.TryGetValue("IsMultiDestination", out var multi) && multi is bool md)
            accomResult.IsMultiDestination = md;

        return accomResult;
    }

    public bool IsMultiDestination(AccommodationResult result)
    {
        return result.IsMultiDestination;
    }

    public void SetNonGroupHotelCount()
    {
        var list = _dataAccess.GetAccommodationList(_context.ParticipantID);
        _context.NonGroupHotelCount = list.Count(a =>
            a.TryGetValue("IsGroupHotel", out var isGroup) && isGroup is false);
    }

    public int GetPrimaryHotelRoomBlockID()
    {
        var data = _dataAccess.GetAccommodationDetails(_context.ParticipantID);
        if (data != null && data.TryGetValue("PrimaryHotelRoomBlockID", out var id) && id is int blockId)
            return blockId;
        return 0;
    }
}
