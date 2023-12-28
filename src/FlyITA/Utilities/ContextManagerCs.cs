using ITALib;
using PCentralLib.participants;
using System;
using System.Text;
using System.Web;

#region " ### Attendee Type Enum ### "

/// <summary>
/// Enum used to keep track of all attendee types
/// </summary>
/// <remarks>
/// TP - 7/31/2008
/// Add new members to this enum to represent the Attendee Types
///  necessary for your program logic.  Then, add the actual 
///  descriptions to the |AttendeeTypeList| block in 
///  WebRegistration.config to complete the process.
/// </remarks>
public enum AttendeeTypeList
{
    atUndefined
}

#endregion

public static class ContextManager
{
    #region " ### Base Properties ### "

    public static string DateFormat
    {
        get { return "MM/dd/yyyy"; }
    }

    public static string SessionID
    {
        get { return HttpContext.Current.Session.SessionID; }
    }

    public static Int32 PersonID
    {
        get { return Convert.ToInt32(HttpContext.Current.Session["PersonID"]); }
        set { HttpContext.Current.Session["PersonID"] = value; }
    }

    public static Int32 ParticipantID
    {
        get
        {
            int pax_id = Convert.ToInt32(HttpContext.Current.Session["ParticipantID"]);

            if (pax_id == 0 && PersonID > 0)
            {
                PCentralParticipant P = PCentralParticipant.GetByPersonIDProgramID(Utilities.GetEnvironmentConnection(), PersonID, ProgramID);

                if (P != null)
                {
                    pax_id = P.ParticipantID;
                    HttpContext.Current.Session["ParticipantID"] = pax_id;
                }
            }

            return pax_id;
        }
        set { HttpContext.Current.Session["ParticipantID"] = value; }
    }

    public static Int32 PartyID
    {
        get
        {
            int partyid = Convert.ToInt32(HttpContext.Current.Session["PartyID"]);

            if (partyid == 0 && ParticipantID > 0)
            {
                partyid = PCentralLib.parties.PCentralParty.GetPartyIDByParticipantID(Utilities.GetEnvironmentConnection(), ParticipantID);
                HttpContext.Current.Session["PartyID"] = partyid;
            }

            return partyid;
        }
        set { HttpContext.Current.Session["PartyID"] = value; }
    }

    public static Int32 CurrentGuestParticipantID
    {
        get { return Convert.ToInt32(HttpContext.Current.Session["CurrentGuestParticipantID"]); }
        set { HttpContext.Current.Session["CurrentGuestParticipantID"] = value; }
    }

    public static bool IsRegistered
    {
        get { return Convert.ToBoolean(HttpContext.Current.Session["IsRegistered"]); }
        set { HttpContext.Current.Session["IsRegistered"] = value; }
    }

    public static Int32 ReportID
    {
        get { return Convert.ToInt32(HttpContext.Current.Session["ReportID"]); }
        set { HttpContext.Current.Session["ReportID"] = value; }
    }

    public static Int32 CultureID
    {
        get { return 1033; }
    }

    public static Int32 RealUserID
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty((HttpContext.Current.Session["RealUserID"])))
            {

                return (int)PCentralLib.UserDefinedEnumerations.EnumPrototypeUserID.Participant;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["RealUserID"]);
            }
        }
        set { HttpContext.Current.Session["RealUserID"] = value; }
    }

    public static Int32 GuestMainCount
    {
        get
        {
            if (HttpContext.Current.Session["GuestMainCount"] == null)
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["GuestMainCount"]);
            }
        }
        set { HttpContext.Current.Session["GuestMainCount"] = value; }
    }
    public static Int32 GuestAdditionalCount
    {
        get
        {
            if (HttpContext.Current.Session["GuestAdditionalCount"] == null)
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["GuestAdditionalCount"]);
            }
        }
        set { HttpContext.Current.Session["GuestAdditionalCount"] = value; }
    }
    public static Int32 GuestNotParticipatingCount
    {
        get
        {
            if (HttpContext.Current.Session["GuestNotParticipatingCount"] == null)
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["GuestNotParticipatingCount"]);
            }
        }
        set { HttpContext.Current.Session["GuestNotParticipatingCount"] = value; }
    }
    public static bool PersonMatching
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["PersonMatching"]))
            {
                return Convert.ToBoolean(Utilities.ReadCustomConfigValue("pages/selfenroll.aspx", "option_person_match", "true"));
            }
            else
            {
                return (bool)HttpContext.Current.Session["PersonMatching"];
            }
        }
        set { HttpContext.Current.Session["PersonMatching"] = value; }
    }

    public static int AllowedNumberOfGuests
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["AllowedNumberOfGuests"]))
            {
                return Convert.ToInt16(Utilities.ReadCustomConfigValue("guests/main", "value", "0")) + Convert.ToInt16(Utilities.ReadCustomConfigValue("guests/additional", "value", "0")) + Convert.ToInt16(Utilities.ReadCustomConfigValue("guests/nonparticipating", "value", "0"));
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["AllowedNumberOfGuests"]);
            }
        }
        set { HttpContext.Current.Session["AllowedNumberOfGuests"] = value; }
    }

    public static Int32 ActingAsUserID
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["ActingAsUserID"]))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["ActingAsUserID"]);
            }
        }
        set { HttpContext.Current.Session["ActingAsUserID"] = value; }
    }

    public static string CMSUser
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["CMSUser"]))
            {
                return string.Empty;
            }
            else
            {
                return (string)HttpContext.Current.Session["CMSUser"];
            }
        }
        set { HttpContext.Current.Session["CMSUser"] = value; }
    }

    public static int ProgramID
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["ProgramID"]) || (Int32)HttpContext.Current.Session["ProgramID"] == 0)
            {
                // setting ProgramID in the config overrides everything
                if (Convert.ToInt32(Utilities.ReadCustomConfigValue("ProgramID", "value", "0")) > 0)
                {
                    HttpContext.Current.Session["ProgramID"] = Convert.ToInt32(Utilities.ReadCustomConfigValue("ProgramID", "value", "0"));
                }
                else
                {
                    HttpContext.Current.Session["ProgramID"] = "FlyITA";
                }

                return 0;
            }
            else
            {
                return (int)HttpContext.Current.Session["ProgramID"];
            }
        }
    }

    public static string TransportationType
    {
        get { return Convert.ToString(HttpContext.Current.Session["TransportType"]); }
        set { HttpContext.Current.Session["TransportType"] = value.ToLower(); }
    }

    public static System.DateTime TravelDateBeginOA
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["TravelDateBeginOA"]); }
        set { HttpContext.Current.Session["TravelDateBeginOA"] = value; }
    }

    public static System.DateTime TravelDateEndOA
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["TravelDateEndOA"]); }
        set { HttpContext.Current.Session["TravelDateEndOA"] = value; }
    }

    public static bool OvernightInFlight
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["OvernightInFlight"]))
            {
                //Get the value from config file
                return Convert.ToBoolean(Utilities.ReadCustomConfigValue("pages/transportationselection.aspx", "Overnight_InFlight", "false"));
            }
            else
            {
                return (bool)HttpContext.Current.Session["OvernightInFlight"];
            }
        }
        set { HttpContext.Current.Session["OvernightInFlight"] = value; }
    }

    public static string TravelExtension
    {
        get { return Convert.ToString(HttpContext.Current.Session["TravelExtension"]); }
        set { HttpContext.Current.Session["TravelExtension"] = value; }
    }
    public static System.DateTime TravelDateBegin
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["TravelDateBegin"]); }
        set { HttpContext.Current.Session["TravelDateBegin"] = value; }
    }
    public static System.DateTime TravelDateEnd
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["TravelDateEnd"]); }
        set { HttpContext.Current.Session["TravelDateEnd"] = value; }
    }
    public static System.DateTime CruiseDateBegin
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["CruiseDateBegin"]); }
        set { HttpContext.Current.Session["CruiseDateBegin"] = value; }
    }
    public static System.DateTime CruiseDateEnd
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["CruiseDateEnd"]); }
        set { HttpContext.Current.Session["CruiseDateEnd"] = value; }
    }
    public static bool GroupHotelDateSet
    {
        get { return Convert.ToBoolean(HttpContext.Current.Session["GroupHotelDateSet"]); }
        set { HttpContext.Current.Session["GroupHotelDateSet"] = value; }
    }
    public static System.DateTime GroupHotelDateEnd
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["GroupHotelDateEnd"]); }
        set
        {
            HttpContext.Current.Session["GroupHotelDateEnd"] = value;
            if (GroupHotelDateEnd > System.DateTime.MinValue && value > System.DateTime.MinValue)
            {
                GroupHotelDateSet = true;
            }
        }
    }
    public static System.DateTime GroupHotelDateBegin
    {
        get { return Convert.ToDateTime(HttpContext.Current.Session["GroupHotelDateBegin"]); }
        set
        {
            HttpContext.Current.Session["GroupHotelDateBegin"] = value;
            if (value > System.DateTime.MinValue && ContextManager.GroupHotelDateEnd > System.DateTime.MinValue)
            {
                ContextManager.GroupHotelDateSet = true;
            }
        }
    }
    public static string PartyRegistrationStatus
    {
        get
        {
            if (HttpContext.Current.Session["PartyRegistrationStatus"] != null)
            {
                return (string)HttpContext.Current.Session["PartyRegistrationStatus"];
            }
            else
            {
                HttpContext.Current.Session["PartyRegistrationStatus"] = Utilities.ReadPartyRegistrationStatus();
                return (string)HttpContext.Current.Session["PartyRegistrationStatus"];
            }
        }
        set { HttpContext.Current.Session["PartyRegistrationStatus"] = value; }
    }
    public static string LoginForSelfEnrollPassword
    {
        get
        {
            if (HttpContext.Current.Session["LoginForSelfEnrollPassword"] != null)
            {
                return (string)HttpContext.Current.Session["LoginForSelfEnrollPassword"];
            }
            else
            {
                return "";
            }
        }
        set { HttpContext.Current.Session["LoginForSelfEnrollPassword"] = value; }
    }
    public static string LevelAchieved
    {
        get
        {
            if (HttpContext.Current.Session["LevelAchieved"] != null)
            {
                return (string)HttpContext.Current.Session["LevelAchieved"];
            }
            else
            {
                return "";
            }
        }
        set { HttpContext.Current.Session["LevelAchieved"] = value; }
    }
    public static string ExistingPhoto
    {
        get
        {
            if (HttpContext.Current.Session["ExistingPhoto"] != null)
            {
                return (string)HttpContext.Current.Session["ExistingPhoto"];
            }
            else
            {
                return "";
            }
        }
        set { HttpContext.Current.Session["ExistingPhoto"] = value; }
    }
    public static string LoginType
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["LoginType"]))
            {
                return Utilities.ReadCustomConfigValue("pages/login.aspx", "option_login_type", "normal");
            }
            else
            {
                return (string)HttpContext.Current.Session["LoginType"];
            }
        }
        set { HttpContext.Current.Session["LoginType"] = value; }
    }
    public static string CFNameSeamlessLogin
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["CFNameSeamlessLogin"]))
            {
                return Utilities.ReadCustomConfigValue("pages/login.aspx", "option_custom_field_for_seamless", "");
            }
            else
            {
                return (string)HttpContext.Current.Session["CFNameSeamlessLogin"];
            }
        }
        set { HttpContext.Current.Session["CFNameSeamlessLogin"] = value; }
    }
    public static string RegistrationClosedType
    {
        get
        {
            if (HttpContext.Current.Session["RegistrationClosedType"] != null)
            {
                return (string)HttpContext.Current.Session["RegistrationClosedType"];
            }

            return "";

        }
        set { HttpContext.Current.Session["RegistrationClosedType"] = value; }
    }


    public static string FeeDisplayTravelType
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["FeeDisplayTravelType"]))
            {
                HttpContext.Current.Session["FeeDisplayTravelType"] = FeesDisplay.GetFeeDisplayTravelType();
                return (string)HttpContext.Current.Session["FeeDisplayTravelType"];
            }
            else
            {
                return (string)HttpContext.Current.Session["FeeDisplayTravelType"];
            }
        }
        set { HttpContext.Current.Session["FeeDisplayTravelType"] = value; }
    }

    public static bool FeeDisplayRentalCar
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["FeeDisplayRentalCar"]))
            {
                HttpContext.Current.Session["FeeDisplayRentalCar"] = FeesDisplay.GetFeeDisplayRentalCar();
                return (bool)HttpContext.Current.Session["FeeDisplayRentalCar"];
            }
            else
            {
                return (bool)HttpContext.Current.Session["FeeDisplayRentalCar"];
            }
        }
        set { HttpContext.Current.Session["FeeDisplayRentalCar"] = value; }
    }

    public static bool FeeDisplayExtending
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["FeeDisplayExtending"]))
            {
                HttpContext.Current.Session["FeeDisplayExtending"] = FeesDisplay.GetFeeDisplayExtending();
                return (bool)HttpContext.Current.Session["FeeDisplayExtending"];
            }
            else
            {
                return (bool)HttpContext.Current.Session["FeeDisplayExtending"];
            }
        }
        set { HttpContext.Current.Session["FeeDisplayExtending"] = value; }
    }

    public static bool FeeDisplayExtendingGroup
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["FeeDisplayExtendingGroup"]))
            {
                HttpContext.Current.Session["FeeDisplayExtendingGroup"] = FeesDisplay.GetFeeDisplayExtendingGroup();
                return (bool)HttpContext.Current.Session["FeeDisplayExtendingGroup"];
            }
            else
            {
                return (bool)HttpContext.Current.Session["FeeDisplayExtendingGroup"];
            }
        }
        set { HttpContext.Current.Session["FeeDisplayExtendingGroup"] = value; }
    }

    public static bool FeeDisplayAccomAssistance
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["FeeDisplayAccomAssistance"]))
            {
                HttpContext.Current.Session["FeeDisplayAccomAssistance"] = FeesDisplay.GetFeeDisplayAccomAssistance();
                return (bool)HttpContext.Current.Session["FeeDisplayAccomAssistance"];
            }
            else
            {
                return (bool)HttpContext.Current.Session["FeeDisplayAccomAssistance"];
            }
        }
        set { HttpContext.Current.Session["FeeDisplayAccomAssistance"] = value; }
    }


    public static new string ToString
    {
        get
        {
            StringBuilder sb = default(StringBuilder);
            sb = new StringBuilder();

            sb.Append("SessionID = ");
            sb.Append(SessionID);
            sb.Append("; ");
            sb.Append("PersonID = ");
            sb.Append(PersonID);
            sb.Append("; ");
            sb.Append("ParticipantID = ");
            sb.Append(ParticipantID);
            sb.Append("; ");
            sb.Append("PartyID = ");
            sb.Append(PartyID);
            sb.Append("; ");
            sb.Append("CurrentGuestParticipantID = ");
            sb.Append(CurrentGuestParticipantID);
            sb.Append("; ");
            sb.Append("IsRegistered = ");
            sb.Append(IsRegistered);
            sb.Append("; ");
            sb.Append("CultureID = ");
            sb.Append(CultureID);
            sb.Append("; ");
            sb.Append("RealUserID = ");
            sb.Append(RealUserID);
            sb.Append("; ");
            sb.Append("ActingAsUserID = ");
            sb.Append(ActingAsUserID);
            sb.Append("; ");
            sb.Append("CMSUser = ");
            sb.Append(CMSUser);
            sb.Append("; ");
            sb.Append("ProgramID = ");
            sb.Append(ProgramID);

            return sb.ToString();
        }
    }

    public static bool? IsAttending
    {
        get
        {
            return get_session_value("wrIsAttending").toBoolNullable();
        }

        set { HttpContext.Current.Session["wrIsAttending"] = value; }
    }

    public static string AttendeeType
    {
        get
        {
            object o = HttpContext.Current.Session["wrAttendeeType"];
            if (Utilities.IsNothingNullOrEmpty(o))
            {
                return "";
            }
            else
            {
                return Convert.ToString(o);
            }
        }
        set { HttpContext.Current.Session["wrAttendeeType"] = value; }
    }


    public static TransportationSection TransportationSection
    {
        get
        {
            object o = HttpContext.Current.Session["wrTransportationSection"];
            if (Utilities.IsNothingNullOrEmpty(o))
            {
                return TransportationSection.TransportationNoSelection;
            }
            else
            {
                return (TransportationSection)o;
            }
        }
        set { HttpContext.Current.Session["wrTransportationSection"] = value; }
    }

    

  
    // ms 5.5.2010 added for PCI
    public static string CardToken
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["wrCardToken"]))
            {
                return (string)HttpContext.Current.Session["wrCardToken"];
            }
            return null;
        }
        set { HttpContext.Current.Session["wrCardToken"] = value; }
    }

    // ms 5.5.2010 Card Web Service request ID
    public static string CardWebCaptureRequestId
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty((HttpContext.Current.Session["wrCardWebCaptureRequestId"])))
            {
                return ((string)HttpContext.Current.Session["wrCardWebCaptureRequestId"]);
            }
            return "";
        }
        set { HttpContext.Current.Session["wrCardWebCaptureRequestId"] = value; }
    }

    public static Int32 NonGroupHotelCount
    {
        get
        {
            if (Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["NonGroupHotelCount"]))
            {
                return -1;
            }
            else
            {
                return Convert.ToInt32(HttpContext.Current.Session["NonGroupHotelCount"]);
            }
        }
        set { HttpContext.Current.Session["NonGroupHotelCount"] = value; }
    }

    public static object get_session_value(string id)
    {
        if (HttpContext.Current == null || HttpContext.Current.Session == null) return null;
        if (HttpContext.Current.Session[id] == null) return null;
        return HttpContext.Current.Session[id];
    }

    // Used for SAML SSO if you have one
    public static string SAMLSessionID
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["SAMLSessionID"]))
            {
                return HttpContext.Current.Session["SAMLSessionID"].ToString();
            }
            return null;
        }
        set { HttpContext.Current.Session["SAMLSessionID"] = value; }
    }
    #endregion

    #region " ### Custom Properties ### "
    public static Int32 PassengerCount
    {
        get { return Convert.ToInt32(HttpContext.Current.Session["PassengerCount"]); }
        set { HttpContext.Current.Session["PassengerCount"] = value; }
    }
    public static Int32 MainGuestID
    {
        get { return Convert.ToInt32(HttpContext.Current.Session["MainGuestID"]); }
        set { HttpContext.Current.Session["MainGuestID"] = value; }
    }

    //sets mobile type if request is from mobile device
    public static string MobileType
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["MobileType"]))
            {
                return (string)HttpContext.Current.Session["MobileType"];
            }
            return "";
        }
        set { HttpContext.Current.Session["MobileType"] = value; }
    }

    public static bool ReturnFromMobileDevice
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["ReturnFromMobileDevice"]))
            {
                return (bool)HttpContext.Current.Session["ReturnFromMobileDevice"];
            }
            return false;
        }
        set { HttpContext.Current.Session["ReturnFromMobileDevice"] = value; }
    }
    public static bool IsSelfEnroll
    {
        get
        {
            if (!Utilities.IsNothingNullOrEmpty(HttpContext.Current.Session["IsSelfEnroll"]))
            {
                return (bool)HttpContext.Current.Session["IsSelfEnroll"];
            }
            return false;
        }
        set { HttpContext.Current.Session["IsSelfEnroll"] = value; }
    }
    #endregion
}
