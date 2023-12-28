using PCentralLib;
using PCentralLib.accommodations;
using PCentralLib.custom_fields;
using PCentralLib.participants;
using PCentralLib.parties;
using PCentralLib.programs;
using PCentralLib.WebReg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

public static class AccommodationsNav
{
    public static void NextPage(Label ErrorLabel)
    {
        WebRegAccommodationResult AccomResult = GetAccommodationDetails(ref ErrorLabel);

        if (IsMultiDestination(ref AccomResult))
        {
            Utilities.Navigate(Utilities.NextPage("multi_hotel"));
        }
        else if (AccomResult.HotelRoomBlocks.Select(string.Concat("InventoryBlockType = ", Convert.ToInt32(PCentralLib.Enumerations.enumInventoryBlockTypes.CruiseCabinBlock).ToString())).Length > 0)
        {
            // Cruise Cabin Block Exists

            // We'll calculate this when it happens
            // STORE EXTENSION IN SESSION, WILL BE USED/ACCESSED IN CRUISE SAVE BUTTON CLICK
            if (AccomResult.TransportationBeginDT < AccomResult.DestinationBeginDT || AccomResult.TransportationEndDT > AccomResult.DestinationEndDT)
            {
                ContextManager.TravelExtension = "true";
            }
            else
            {
                ContextManager.TravelExtension = "false";
            }

            Utilities.Navigate(Utilities.NextPage("cruise"));
        }
        else
        {
            ///OVERNIGHT IN-FLIGHT (7/28/08)
            // Extension for overnight in-flight program with ITA air as the transportation type..
            if (ContextManager.OvernightInFlight && string.Compare(ContextManager.TransportationType, "ita", true) == 0)
            {
                if (AccomResult.TransportationBeginDT.AddDays(1) < AccomResult.DestinationBeginDT || AccomResult.TransportationEndDT > AccomResult.DestinationEndDT)
                {
                    ContextManager.TravelExtension = "true";
                    Utilities.Navigate(Utilities.NextPage("hotel_extension"));
                }
                else
                {
                    Utilities.Navigate(Utilities.NextPage());
                }
            }
            else
            {
                if (AccomResult.TransportationBeginDT < AccomResult.DestinationBeginDT || AccomResult.TransportationEndDT > AccomResult.DestinationEndDT)
                {
                    ContextManager.TravelExtension = "true";
                    Utilities.Navigate(Utilities.NextPage("hotel_extension"));
                }
                else
                {
                    Utilities.Navigate(Utilities.NextPage());
                }
            }
        }
    }

    public static void NextPageCruise(Label ErrorLabel)
    {
        if (!(ContextManager.TravelExtension.Length > 0 && ContextManager.TravelDateBegin > System.DateTime.MinValue && ContextManager.TravelDateEnd > System.DateTime.MinValue))
        {
            SetTransportationDates();
        }

        if (!(ContextManager.CruiseDateBegin > System.DateTime.MinValue && ContextManager.CruiseDateEnd > System.DateTime.MinValue))
        {
            SetCruiseDates();
        }

        if (ContextManager.TravelExtension.Length > 0 && ContextManager.TravelDateBegin > System.DateTime.MinValue && ContextManager.TravelDateEnd > System.DateTime.MinValue)
        {
            ///OVERNIGHT IN-FLIGHT 
            // Extension for overnight in-flight with ITA air as the transportation type.
            if (ContextManager.OvernightInFlight && string.Compare(ContextManager.TransportationType, "ita", true) == 0)
            {
                if (ContextManager.TravelDateBegin.AddDays(1) < ContextManager.CruiseDateBegin || ContextManager.TravelDateEnd > ContextManager.CruiseDateEnd)
                {
                    ContextManager.TravelExtension = "true";
                    Utilities.Navigate(Utilities.NextPage("hotel_extension"));
                }
                else
                {
                    Utilities.Navigate(Utilities.NextPage());
                }
            }
            else
            {
                if (ContextManager.TravelDateBegin == ContextManager.CruiseDateBegin && ContextManager.TravelDateEnd == ContextManager.CruiseDateEnd)
                {
                    // cruise only
                    Utilities.Navigate(Utilities.NextPage());
                }
                else
                {
                    if (string.Compare(ContextManager.TravelExtension, "true", true) == 0)
                    {
                        Utilities.Navigate(Utilities.NextPage("hotel_extension"));
                    }
                    else
                    {
                        Utilities.Navigate(Utilities.NextPage("hotel_preferences"));
                    }
                }
            }
        }
    }

    public static bool IsMultiDestination(ref WebRegAccommodationResult AccomResult)
    {
        ArrayList arrProgDest = default(ArrayList);

        arrProgDest = new ArrayList();
        foreach (DataRow drDest in AccomResult.HotelRoomBlocks.Rows)
        {
            if ((PCentralLib.Enumerations.enumInventoryBlockTypes)drDest["InventoryBlockType"] == PCentralLib.Enumerations.enumInventoryBlockTypes.HotelRoomBlock)
            {
                if (!arrProgDest.Contains(drDest["ProgramDestinationID"]))
                {
                    arrProgDest.Add(drDest["ProgramDestinationID"]);
                }
            }
        }

        return arrProgDest.Count > 1;
    }

    public static void SetTransportationDates(System.Web.UI.WebControls.Label ErrorLabel = null)
    {
        WebRegAccommodationResult AccomResult = default(WebRegAccommodationResult);

        AccomResult = GetAccommodationDetails(ref ErrorLabel);

        ContextManager.TravelDateBegin = AccomResult.TransportationBeginDT;
        ContextManager.TravelDateEnd = AccomResult.TransportationEndDT;

        if (AccomResult.TransportationBeginDT < AccomResult.DestinationBeginDT || AccomResult.TransportationEndDT > AccomResult.DestinationEndDT)
        {
            ContextManager.TravelExtension = "true";
        }
        else
        {
            ContextManager.TravelExtension = "false";
        }
    }

    public static void SetCruiseDates()
    {
        WebRegCruiseResult AccomResult = GetCruiseDetails();
        Int32 lngCruiseCabinBlockID = default(Int32);

        if (AccomResult.AccommodationID == 0)
        {
            lngCruiseCabinBlockID = GetCruiseCabinBlockID(AccomResult.CruiseShipBlocks);
        }
        else if (AccomResult.AccomCabin != null)
        {
            lngCruiseCabinBlockID = AccomResult.AccomCabin.CruiseCabinBlockID;
        }

        DataTable dt = AccomResult.CruiseShipBlockItems;
        DateTime dti = ContextManager.CruiseDateBegin;
        DateTime dt2 = ContextManager.CruiseDateEnd;
        GetShipSailingDates(ref dt, lngCruiseCabinBlockID, ref dti, ref dt2);
    }

    private static WebRegCruiseResult GetCruiseDetails()
    {
        return WebRegAccommodationFacade.ReadCruiseData(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);
    }

    public static void SetGroupHotelDates(WebRegAccommodationResult AccomResult)
    {
        Hashtable hashDest = default(Hashtable);
        Hashtable hashHRB = default(Hashtable);
        string strHRBIDs = null;
        string[] HRBIDs = null;
        PCentralEntityList<PCentralCustomFieldMapAccom> CustomFieldMapAccom;
        DataSet dsPartCFields = null;
        SortedList sortHRB = default(SortedList);
        ArrayList IDs = default(ArrayList);
        ArrayList arrDates = default(ArrayList);
        DataTable refdt;

        //If IsMultiDestination(AccomResult) Then
        ///GET HOTELROOMBLOCKS MAPPING

        PCentralCustomFieldDrivers cf_drivers = new PCentralCustomFieldDrivers(Utilities.GetEnvironmentConnection());
        CustomFieldMapAccom = ReadMappedAccomToCustomField();

        ///GET HOTELROOMBLOCKID MAPPED TO CUSTOMFIELD

        if (cf_drivers.AccomCustomFieldID > 0)
        {
            dsPartCFields = PCentralProgramCustomField.ReadParticipantCustomFieldsByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);
        }

        hashDest = new Hashtable();
        foreach (DataRow drDest in AccomResult.HotelRoomBlocks.Rows)
        {
            if (hashDest.ContainsKey(drDest["ProgramDestinationID"]))
            {
                hashDest[drDest["ProgramDestinationID"]] = string.Concat(hashDest[drDest["ProgramDestinationID"]], ",", drDest["HotelRoomBlockID"]);
            }
            else
            {
                hashDest.Add(drDest["ProgramDestinationID"], drDest["HotelRoomBlockID"]);
            }
        }

        IDs = new ArrayList();
        hashHRB = new Hashtable();
        foreach (int Key in hashDest.Keys)
        {
            strHRBIDs = hashDest[Key].ToString();
            HRBIDs = strHRBIDs.Split(new[] { ',' });

            // CHECK IF THIS PARTICIPANT IS GOING TO THIS DESTINATION BY ASSIGNED DATES
            if (CheckParticipantDestinationByDefaultDates(ref AccomResult, ref HRBIDs))
            {
                if (HRBIDs.Length == 1)
                {
                    // SINGLE HOTEL FOR SINGLE DESTINATION
                    IDs.Add(HRBIDs[0]);
                }
                else
                {
                    // MULTI HOTELS FOR SINGLE DESTINATIONS, GO GET THE RIGHT ONE
                    IDs.Add(GetHotelRoomBlockIDByProgramDestinationID(ref AccomResult, Key, ref CustomFieldMapAccom, ref dsPartCFields, ref hashHRB));
                }
            }
        }

        sortHRB = new SortedList();
        foreach (Int32 HRBID in IDs)
        {
            refdt = AccomResult.PartyDefaultDestinationDates.Tables[0];
            arrDates = WebRegAccommodationFacade.GetDefaultDestDatesByHotelRoomBlockID_AL(refdt, HRBID);
            if (arrDates.Count > 0)
            {
                if (!sortHRB.ContainsKey(arrDates[0]))
                {
                    sortHRB.Add(arrDates[0], HRBID);
                }
            }
        }

        foreach (WebRegGroupHotelRoom room in AccomResult.GroupHotelRoomlist)
        {
            DataSet dt;

            if (room.AccomGroupHotel.HotelRoomBlockID == Convert.ToInt32(sortHRB.GetByIndex(0)))
            {
                // first group hotel
                // get the smallest room number(or accommodation)
                dt = room.AccomParticipants;
                if (PrimaryParticipantExists(ref dt))
                {
                    ContextManager.GroupHotelDateBegin = room.AccomGroupHotel.RoomCheckInDT;
                    break;
                }
            }
        }

        foreach (WebRegGroupHotelRoom room in AccomResult.GroupHotelRoomlist)
        {
            DataSet dt;

            if (room.AccomGroupHotel.HotelRoomBlockID == Convert.ToInt32(sortHRB.GetByIndex(sortHRB.Count - 1)))
            {
                dt = room.AccomParticipants;
                // last group hotel
                // get the smallest room number(or accommodation)
                if (PrimaryParticipantExists(ref dt))
                {
                    ContextManager.GroupHotelDateEnd = room.AccomGroupHotel.RoomCheckOutDT;
                    break;
                }
            }
        }

        ContextManager.GroupHotelDateSet = true;
        //End If
    }

    private static bool PrimaryParticipantExists(ref DataSet AccomParticipants)
    {
        foreach (DataRow dr in AccomParticipants.Tables[0].Rows)
        {
            if (Convert.ToInt32(dr["ParticipantID"]) == ContextManager.ParticipantID)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetHotelRoomBlockIDByProgramDestinationID(ref WebRegAccommodationResult AccomResult, int ProgramDestinationID, ref PCentralEntityList<PCentralCustomFieldMapAccom> listAccoms, ref DataSet dsCF, ref Hashtable hashHRB)
    {
        SortedList sort = default(SortedList);

        int HotelRoomBlockID = SetContractedRoomType(ref AccomResult, ProgramDestinationID, ref listAccoms, ref dsCF, ref hashHRB);

        // GET THE LOWEST HotelRoomBlockID
        if (HotelRoomBlockID == 0 && AccomResult.HotelRoomBlocks.Rows.Count > 0)
        {
            sort = GetSortedHotelRoomBlockIDs(ref AccomResult, ProgramDestinationID);

            if (sort.Count > 0)
            {
                HotelRoomBlockID = Convert.ToInt32(sort.GetByIndex(0));
            }
        }

        return HotelRoomBlockID;
    }

    private static int SetContractedRoomType(ref WebRegAccommodationResult AccomResult, int ProgramDestinationID, ref PCentralEntityList<PCentralCustomFieldMapAccom> listAccoms, ref DataSet dsCF, ref Hashtable hashHRB)
    {
        int HotelRoomBlockID = 0;
        SortedList sort = default(SortedList);

        // GET HOTELROOMBLOCKID MAPPED TO CUSTOMFIELD
        if (dsCF != null)
        {
            foreach (PCentralCustomFieldMapAccom CFMAEnty in listAccoms)
            {
                // CHECK FOR CURRENT PROGRAM DESTINATION ID BEFORE CHECKING CUSTOM FIELD VALUES
                // GET HOTELS FOR THIS PROGRAM DESTINATION  -- UNTIL PROGRAM DESTINATIONID IS SUPPORTED IN METADATA SERVICES
                sort = GetSortedHotelRoomBlockIDs(ref AccomResult, ProgramDestinationID);

                if (sort.ContainsKey(CFMAEnty.HotelRoomBlockID))
                {
                    foreach (DataRow dr in dsCF.Tables[1].Rows)
                    {
                        if (CFMAEnty.CustomFieldPossibleValueID == Convert.ToInt32(Utilities.NZ(dr["CustomFieldPossibleValueID"], -1024)))
                        {
                            if (!hashHRB.ContainsKey(CFMAEnty.HotelRoomBlockID))
                            {
                                hashHRB.Add(CFMAEnty.HotelRoomBlockID, CFMAEnty.ContractedRoomTypeID);
                                HotelRoomBlockID = CFMAEnty.HotelRoomBlockID;
                                break;
                            }
                        }
                    }

                    // FOUND MAPPED HOTELROOMBLOCK AND ROOMTYPE
                    if (HotelRoomBlockID > 0)
                    {
                        break;
                    }
                }
            }
        }

        return HotelRoomBlockID;
    }

    private static SortedList GetSortedHotelRoomBlockIDs(ref WebRegAccommodationResult AccomResult, int ProgramDestinationID)
    {
        SortedList sort = new SortedList();
        DataRow[] rows = AccomResult.HotelRoomBlocks.Select(string.Concat("InventoryBlockType = ", Convert.ToInt32(PCentralLib.Enumerations.enumInventoryBlockTypes.HotelRoomBlock), " AND ProgramDestinationID = ", ProgramDestinationID));

        foreach (DataRow dr in rows)
        {
            if (!sort.Contains(dr["HotelRoomBlockID"]))
            {
                sort.Add(dr["HotelRoomBlockID"], rows[0]["HotelRoomBlockID"]);
            }
        }

        return sort;
    }

    private static bool CheckParticipantDestinationByDefaultDates(ref WebRegAccommodationResult AccomResult, ref string[] HRBIDs)
    {
        List<DateTime> dates;
        DataTable dt = AccomResult.PartyDefaultDestinationDates.Tables[0];

        foreach (string HRBID in HRBIDs)
        {
            // CHECK IF THIS PARTICIPANT IS GOING TO THIS DESTINATION BY ASSIGNED DATES
            dates = WebRegAccommodationFacade.GetDefaultDestDatesByHotelRoomBlockID(dt, Convert.ToInt32(HRBID));

            // HE'S NOT GOING TO THIS HOTELROOMBLOCK DESTINATION
            if (dates.Count == 0)
            {
                // DO NOT ADD THIS HOTELROOMBLOCK FOR THIS DESTINATION
                return false;
            }
        }
        return true;
    }

    private static PCentralEntityList<PCentralCustomFieldMapAccom> ReadMappedAccomToCustomField()
    {
        PCentralEntityList<PCentralCustomFieldMapAccom> CustomFieldMapAccomList = new PCentralEntityList<PCentralCustomFieldMapAccom>();
        PCentralCustomFieldDrivers cf_drivers = null;

        try
        {
            cf_drivers = PCentralCustomFieldDrivers.GetByProgramID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID);

            foreach (PCentralCustomFieldMapAccom ea in cf_drivers.CustomFieldMapAccom)
            {
                CustomFieldMapAccomList.Add(ea);
            }

            return CustomFieldMapAccomList;
        }
        finally
        {
        }
    }

    private static int GetCruiseCabinBlockID(DataTable dtBlocks)
    {
        DataRow[] dr = dtBlocks.Select(string.Concat("InventoryBlockType = ", Convert.ToInt32(PCentralLib.Enumerations.enumInventoryBlockTypes.CruiseCabinBlock)), "InventoryBlockID ASC");

        if (dr.Length > 0)
        {
            return Convert.ToInt32(dr[0]["InventoryBlockID"]);
        }
        else
        {
            return 0;
        }
    }

    private static void GetShipSailingDates(ref DataTable dt, Int32 CruiseCabinBlockID, ref  System.DateTime dteBeginDate, ref System.DateTime dteEndDate)
    {
        DataRow[] dr = dt.Select(string.Concat("CruiseCabinBlockID = ", CruiseCabinBlockID));

        if (dr.Length > 0)
        {
            dteBeginDate = Convert.ToDateTime(dr[0]["ShipSailingBeginDT"]);
            dteEndDate = Convert.ToDateTime(dr[0]["ShipSailingEndDT"]);
        }
    }

    private static WebRegAccommodationResult GetAccommodationDetails(ref Label ErrorLabel)
    {
        List<DateTime> dest_dates = null;
        DataTable dt;
        WebRegAccommodationResult WRAResults = WebRegAccommodationFacade.ReadAccommodation(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);
        WRAResults.SetPrimaryParticipantGroupHotel(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

        //GET PRIMARY HOTEL ROOM BLOCK ID
        if (WRAResults.AccomGroupHotel.AccommodationID == 0)
        {
            dt = WRAResults.HotelRoomBlocks;
            WRAResults.HotelRoomBlockID = GetPrimaryHotelRoomBlockID(ref dt);
        }
        else
        {
            WRAResults.HotelRoomBlockID = WRAResults.AccomGroupHotel.HotelRoomBlockID;
        }

        dt = WRAResults.PartyDefaultDestinationDates.Tables[0];
        dest_dates = WebRegAccommodationFacade.GetDefaultDestDatesByHotelRoomBlockID(dt, WRAResults.HotelRoomBlockID);

        if (dest_dates.Count > 0)
        {
            WRAResults.DestinationBeginDT = dest_dates[0];
            WRAResults.DestinationEndDT = dest_dates[1];
        }
        else if (WRAResults.HotelRoomBlocks.Select(string.Concat("InventoryBlockType = ", Convert.ToInt32(PCentralLib.Enumerations.enumInventoryBlockTypes.CruiseCabinBlock).ToString())).Length > 0)
        {
            //this is a CRUISE
            if (!(ContextManager.CruiseDateBegin > DateTime.MinValue && ContextManager.CruiseDateEnd > DateTime.MinValue))
            {
                SetCruiseDates();
            }

            WRAResults.DestinationBeginDT = ContextManager.CruiseDateBegin;
            WRAResults.DestinationEndDT = ContextManager.CruiseDateEnd;
        }

        // SET TRANSPORTATION DATES
        WRAResults.SetTransportationBeginEndDates();

        return WRAResults;
    }

    public static int GetPrimaryHotelRoomBlockID(ref DataTable dt)
    {
        DataRow[] drs = null;

        if (dt.Rows.Count > 0)
        {
            drs = dt.Select(string.Concat("InventoryBlockType = ", Convert.ToInt32(Enumerations.enumInventoryBlockTypes.HotelRoomBlock).ToString(), " AND PrimaryFlg = 1"));
            if (drs.Length > 0)
            {
                return Convert.ToInt32(drs[0]["HotelRoomBlockID"]);
            }
        }

        return 0;
    }

    public static void SetNonGroupHotelCount()
    {
        try
        {
            WebRegAccommodationResult AccomResult = WebRegAccommodationFacade.ReadAccommodation(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);
            ContextManager.NonGroupHotelCount = AccomResult.NonGroupHotelRequests.Tables[0].Rows.Count;
        }
        finally
        {
            //if ((facade != null))
            //    facade.Dispose();
        }
    }

    public static void SaveOverNightInFlight()
    {
        try
        {
            PCentralAccomOvernightInFlight overnight = new PCentralAccomOvernightInFlight(Utilities.GetEnvironmentConnection());
            PCentralEntityList<PCentralAccomOvernightInFlight> list = new PCentralEntityList<PCentralAccomOvernightInFlight>();

            //Set OnEntity to Add
            if (string.Compare(ContextManager.TransportationType, "own", true) == 0)
            {
                overnight.CheckInDate = ContextManager.TravelDateBeginOA;
                overnight.CheckOutDate = ContextManager.TravelDateBeginOA.AddDays(1);
            }
            else
            {
                overnight.CheckInDate = ContextManager.TravelDateBegin;
                overnight.CheckOutDate = ContextManager.TravelDateBegin.AddDays(1);
            }

            //This value for AccomTypeID is set by the business layer object
            // ONEntity.AccomTypeID = (Int32)(enumAccomType.OvernightInFlight);
            overnight.PartyID = ContextManager.PartyID;
            overnight.Comments = "Created at registration per program ONF setting";
            list.Add(overnight);

            // PCEntityCompareResult CompareResults = listON.FindChanges("Overnight In Flight", ContextManager.RealUserID);
            PCentralSaveResults save_results = list.Save(Guid.NewGuid(), ContextManager.RealUserID);

            if (!save_results.was_successful)
            {
                //lblInfo.Text = save_results.validation_results.get_all_errors_br();
                //lblInfo.CssClass = "warning";
            }
        }
        finally
        {
            // if ((onFacade != null))
            //   onFacade.Dispose();
        }
    }

    /*
    public static void RemoveOnOwnNonGroupAccom(ref WebRegAccommodationResult fc)
    {
        WebRegAccommodationResult AccomResult = default(WebRegAccommodationResult);
        bool blnUnaccountedNights = false;
        AccomResult = WebRegAccommodationFacade.ReadAccommodation(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);
        Label ErrorLabel = new Label();
        blnUnaccountedNights = false;
        AccomResult = GetAccommodationDetails(ref ErrorLabel);
        // REMOVE ANY NON GROUP HOTEL REQUESTS IF THEY EXIST
        //EntityList listNGH = default(EntityList);      
        PCentralEntityList<PCentralAccomPrefNonGroup> listNGH = new PCentralEntityList<PCentralAccomPrefNonGroup>();

        if (!blnUnaccountedNights)
        {
            // SET ALL THE NON GROUP REQUESTS TO REMOVE
            foreach (DataRow drRow in AccomResult.NonGroupHotelRequests.Tables[0].Rows)
            {
                PCentralAccomPrefNonGroup NGEntity = new PCentralAccomPrefNonGroup(Utilities.GetEnvironmentConnection());
                NGEntity.Remove = true;
                NGEntity.PartyID = ContextManager.PartyID;
                NGEntity.AccomPrefNonGroupID = Convert.ToInt32(drRow["AccomPrefNonGroupID"]);
                listNGH.Add(NGEntity);
            }
        }
        //todo ahmed
        //fc.NonGroupHotelRequest = listNGH;

        // REMOVE ANY ON OWN IF THEY EXIST
        //   EntityList listOW = default(EntityList);
        PCentralEntityList<PCentralAccomOnOwn> listOW = new PCentralEntityList<PCentralAccomOnOwn>();

        // SET ALL THE ON OWN TO REMOVE

        foreach (DataRow drRow in AccomResult.OnOwn.Tables[0].Rows)
        {
            PCentralAccomOnOwn OWEntity = new PCentralAccomOnOwn(Utilities.GetEnvironmentConnection());
            OWEntity.Remove = true;
            OWEntity.PartyID = ContextManager.PartyID;
            OWEntity.AccomOnOwnID = Convert.ToInt32(drRow["AccomOnOwnID"]);
            listOW.Add(OWEntity);
        }
        //todo ahmed
        //fc.OnOwn = listOW;        
    }
    */

    public static void RemoveOverNightInFlight()
    {
        DataSet list;
        PCentralAccomOvernightInFlight overnight = null;

        try
        {
            list = PCentralAccomOvernightInFlight.ReadByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

            PCentralEntityList<PCentralAccomOvernightInFlight> listON = new PCentralEntityList<PCentralAccomOvernightInFlight>(); ;

            //Set OnEntity to Remove
            foreach (DataRow drRow in list.Tables[0].Rows)
            {
                overnight = new PCentralAccomOvernightInFlight(Utilities.GetEnvironmentConnection());
                overnight.Remove = true;
                overnight.PartyID = ContextManager.PartyID;
                overnight.AccomOvernightInFlightID = (int)drRow["AccomOvernightInFlightID"];
            }

            listON.Add(overnight);

            // PCEntityCompareResult CompareResults = listON.FindChanges("Remove Overnight In Flight", ContextManager.RealUserID);

            PCentralSaveResults save_results = listON.Save(Guid.NewGuid(), ContextManager.RealUserID);

            if (!save_results.was_successful)
            {
                //this.lblInfo.Text = save_results.validation_results.get_all_errors_br();
                //this.lblInfo.CssClass = "warning";
            }
        }
        finally
        {

        }
    }

}
