using FlyITA.Core.Interfaces;

namespace FlyITA.Core.Services;

public class FeesDisplayService : IFeesDisplayService
{
    private readonly IContextManager _context;

    public FeesDisplayService(IContextManager context)
    {
        _context = context;
    }

    public string GetFeeDisplayTravelType()
    {
        // Returns air travel type based on air preference
        // Legacy: checks if TransportationType is ITAAir
        return _context.TransportationType == "ITAAir" ? "ITAAir" : "unknown";
    }

    public bool GetFeeDisplayRentalCar()
    {
        // Legacy: checks if participant requested rental car assistance
        return _context.FeeDisplayRentalCar;
    }

    public bool GetFeeDisplayExtending()
    {
        // Legacy: checks if travel extends beyond default dates
        return _context.FeeDisplayExtending;
    }

    public bool GetFeeDisplayExtendingGroup()
    {
        // Legacy: checks group hotel date extensions
        return _context.FeeDisplayExtendingGroup;
    }

    public bool GetFeeDisplayAccomAssistance()
    {
        // Legacy: checks non-group hotel accommodations
        return _context.NonGroupHotelCount > 0;
    }
}
