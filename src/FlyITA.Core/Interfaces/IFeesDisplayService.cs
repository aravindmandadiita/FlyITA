namespace FlyITA.Core.Interfaces;

public interface IFeesDisplayService
{
    string GetFeeDisplayTravelType();
    bool GetFeeDisplayRentalCar();
    bool GetFeeDisplayExtending();
    bool GetFeeDisplayExtendingGroup();
    bool GetFeeDisplayAccomAssistance();
}
