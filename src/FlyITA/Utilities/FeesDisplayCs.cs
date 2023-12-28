using PCentralLib.accommodations;
using PCentralLib.participants;
using PCentralLib.parties;
using PCentralLib.WebReg;
using System;
using System.Collections.Generic;
using System.Data;

public static class FeesDisplay
{
    public static string GetFeeDisplayTravelType()
    {
        PCentralAirPreference AirPreferenceITA = null;

        PCentralParty party = PCentralParty.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        AirPreferenceITA = party.PrimaryParticipant.AirPreferenceITA;

        if (AirPreferenceITA != null && AirPreferenceITA.AirPreferenceID > 0)
        {
            return "ITAAir";
        }

        return "unknown";
    }

    public static bool GetFeeDisplayRentalCar()
    {
        // this represents a request for assistance with renting a car
        PCentralRentalCarPreferences car_rental = PCentralRentalCarPreferences.GetByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        return (car_rental != null && car_rental.RentalCarPreferencesID > 0);
    }

    public static bool GetFeeDisplayExtending()
    {
        // represents travel outside of the default dates, aka an extension
        AccommodationsNav.SetTransportationDates();
        return Convert.ToBoolean(ContextManager.TravelExtension);
    }

    public static bool GetFeeDisplayExtendingGroup()
    {
        // an extension of group hotel dates
        List<DateTime> dates = new List<DateTime>();
        DataTable dt_room_blocks;

        WebRegAccommodationResult WRResults = WebRegAccommodationFacade.ReadAccommodation(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

        WRResults.SetPrimaryParticipantGroupHotel(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

        //If Not ContextManager.GroupHotelDateSet Then AccommodationsNav.SetGroupHotelDates()
        if (!ContextManager.GroupHotelDateSet)
        {
            //It is probably Cruise only
            return false;
        }
        /*
        else // why set it if ContextManager.GroupHotelDateSet = true???
        {
            AccommodationsNav.SetGroupHotelDates(WRResults);
        }
        */

        //check for multiple hotels. The 1st hotel which is Primary
        //may have different dates then the participant's destination hotel.
        //GET PRIMARY HOTEL ROOM BLOCK ID
        if (WRResults.AccomGroupHotel.AccommodationID == 0)
        {
            dt_room_blocks = WRResults.HotelRoomBlocks;
            WRResults.HotelRoomBlockID = AccommodationsNav.GetPrimaryHotelRoomBlockID(ref dt_room_blocks);
        }
        else
        {
            WRResults.HotelRoomBlockID = WRResults.AccomGroupHotel.HotelRoomBlockID;
        }

        dt_room_blocks = WRResults.PartyDefaultDestinationDates.Tables[0];
        dates = WebRegAccommodationFacade.GetDefaultDestDatesByHotelRoomBlockID(dt_room_blocks, WRResults.HotelRoomBlockID);

        //check for 1st destination hotel night
        if (dates.Count > 0)
        {
            if (ContextManager.GroupHotelDateBegin < dates[0])
            {
                return true;
            }
        }
        else
        {
            //proceed as normal
            if (ContextManager.GroupHotelDateBegin.Year >= 100)
            {
                if (ContextManager.GroupHotelDateBegin < Convert.ToDateTime(WRResults.PartyDefaultDestinationDates.Tables[0].Compute("MIN(BeginDT)", string.Empty)))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        //check the last night at last destination
        if (ContextManager.GroupHotelDateEnd > Convert.ToDateTime(WRResults.PartyDefaultDestinationDates.Tables[0].Compute("MAX(EndDT)", string.Empty)))
        {
            return true;
        }

        return false;
    }

    public static bool GetFeeDisplayAccomAssistance()
    {
        // represents Non-Group Hotel accommodations, i.e. not Group Hotel and not OnOwn
        if (ContextManager.NonGroupHotelCount == -1)
        {
            AccommodationsNav.SetNonGroupHotelCount();
        }

        return ContextManager.NonGroupHotelCount > 0;
    }

}