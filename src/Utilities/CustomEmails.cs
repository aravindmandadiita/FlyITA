using ITALib;
using PCentralLib;
using PCentralLib.custom_fields;
using PCentralLib.email;
using PCentralLib.participants;
using PCentralLib.parties;
using PCentralLib.person;
using PCentralLib.programs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

public class CustomEmails : PresenterBase
{
    private Label _userMessages;

    #region " ### Constants ### "

    const string mLegalFirstName = "<<LegalFirstName>>";
    const string mLegalMiddleName = "<<LegalMiddleName>>";
    const string mLegalLastName = "<<LegalLastName>>";
    const string mDateOfBirth = "<<DateOfBirth>>";
    const string mGender = "<<Gender>>";
    const string mGuestDateOfBirth = "<<GDateOfBirth>>";
    const string mGuestGender = "<<GGender>>";
    const string mEmailAddress = "<<EmailAddress>>";
    const string mCustomFieldTitle = "<<CustomField.Title>>";
    const string mBusinessPhone = "<<BusinessPhone>>";
    const string mBusinessFax = "<<BusinessFax>>";
    const string mMobilePhone = "<<MobilePhone>>";
    const string mHomePhone = "<<HomePhone>>";
    const string mTransportationType = "<<TransportationType>>";
    const string mTravelDates = "<<TravelDates>>";
    const string mPrefHomeDepartureTime = "<<PrefHomeDepartureTime>>";
    const string mPrefDestDepartureTime = "<<PrefDestDepartureTime>>";
    const string mPrefAirline = "<<PrefAirline>>";
    const string mPrefHomeDepartureCity = "<<PrefHomeDepartureCity>>";
    const string mDriveTime = "<<DriveTime>>";
    const string mSeatPreference = "<<SeatPreference>>";
    const string mAirRemarks = "<<AirRemarks>>";
    const string mFrequentFlyerNumber = "<<FrequentFlyerNumber>>";
    const string mCheckInDate = "<<CheckInDate>>";
    const string mCheckOutDate = "<<CheckOutDate>>";
    const string mWheelchair = "<<Wheelchair>>";
    const string mSpecialMeal = "<<SpecialMeal>>";

    const string mGLegalFirstName = "<<GLegalFirstName>>";
    const string mGLegalMiddleName = "<<GLegalMiddleName>>";
    const string mGLegalLastName = "<<GLegalLastName>>";

    const string mGFrequentFlyerNumber = "<<GFrequentFlyerNumber>>";
    const string mGPrefHomeDepartureCity = "<<GPrefHomeDepartureCity>>";
    const string mGSeatPreference = "<<GSeatPreference>>";
    const string mGWheelchair = "<<GWheelchair>>";
    const string mGSpecialMeal = "<<GSpecialMeal>>";

    const string PARTICIPANT_NAME = "[PARTICIPANT_NAME]";
    const string PARTICIPANT_FIRSTNAME = "[FIRSTNAME]";
    const string PARTICIPANT_LASTNAME = "[LASTNAME]";
    const string PARTICIPANT_EMAIL = "[PARTICIPANT_EMAIL]";
    const string PARTICIPANT_USERNAME = "[PARTICIPANT_USERNAME]";
    const string PARTICIPANT_PASSWORD = "[PARTICIPANT_PASSWORD]";
    const string PROGRAM_NAME = "[PROGRAM_NAME]";
    const string PROGRAM_800NBR = "[PROGRAM_800NBR]";
    const string PROGRAM_HQNAME = "[PROGRAM_HQNAME]";
    const string PROGRAM_URL = "[PROGRAM_URL]";
    const string PROGRAM_EMAIL = "[PROGRAM_EMAIL]";

    const string CF_BEGIN = "<<CustomField.";
    const string CF_END = ">>";

    const string GUEST_BLOCK_BEGIN_GI = "<GuestBlockGI>";
    const string GUEST_BLOCK_END_GI = "</GuestBlockGI>";
    const string GUEST_BLOCK_BEGIN_TI = "<GuestBlockTI>";
    const string GUEST_BLOCK_END_TI = "</GuestBlockTI>";

    #endregion

    #region " ### Public Methods ### "

    /// <summary>
    /// Sends an email that mimics the typical registration acknowledgement.
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <remarks>Use this method if you want to change the default email</remarks>
    public void SendRegistrationConfirmation(Label UserMessages)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("RegistrationConfirmationEmailTemplate");
        StringBuilder emailSubject = new StringBuilder(Utilities.ReadCustomConfigValue("RegistrationConfirmationEmailSubject"));

        formatAndSendCustomEmail(emailTemplate, emailSubject);
    }

    /// <summary>
    /// Sends an email that mimics the typical login credentials.
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <remarks>Use this method if you want to change the default email</remarks>
    public void SendLogonCredentials(Label UserMessages)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("LogonCredentialsEmailTemplate");
        StringBuilder emailSubject = new StringBuilder(Utilities.ReadCustomConfigValue("LogonCredentialsEmailSubject"));

        formatAndSendCustomEmail(emailTemplate, emailSubject);
    }

    /// <summary>
    /// Sends an email that mimics the typical login credentials.
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <remarks>Use this method if you want to change the default email</remarks>
    public string PreviewLogonCredentials(Label UserMessages)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("LogonCredentialsEmailTemplate");
        StringBuilder emailSubject = new StringBuilder(Utilities.ReadCustomConfigValue("LogonCredentialsEmailSubject"));

        return GetCustomEmailPreview(emailTemplate, emailSubject);
    }

    /// <summary>
    /// Sends an email to the user that tells them what their login is.
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <remarks>Use this method if you want to change the default email</remarks>
    public void SendForgotPasswordCredentials(Label UserMessages)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("SeamlessLogonCredentialsEmail");
        StringBuilder emailSubject = new StringBuilder(Utilities.ReadCustomConfigValue("SeamlessLogonCredentialsSubject"));

        formatAndSendCustomEmail(emailTemplate, emailSubject);
    }

    /// <summary>
    /// Sends an email to the user that tells them what their login is based on CF value.
    /// This method is used for internal seamless login type.
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <param name="toEmail"></param>
    /// <param name="firstname"></param>
    /// <param name="lastname"></param>
    /// <param name="password"></param>
    /// <remarks>Sends an email to the user that tells them what their login is based on CF value. This method is used for internal-seamless login type.</remarks>
    public void SendForgotPasswordCredentials(Label UserMessages, string toEmail, string firstname, string lastname, string password)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("SeamlessLogonCredentialsEmail");
        StringBuilder emailSubject = new StringBuilder(Utilities.ReadCustomConfigValue("SeamlessLogonCredentialsSubject"));

        formatAndSendCustomEmail(emailTemplate, emailSubject, toEmail, firstname, lastname, password);
    }

    /// <summary>
    /// Sends the default third party email
    /// </summary>
    /// <param name="UserMessages"></param>
    /// <remarks></remarks>
    public void SendTravelerProfileEmail(Label UserMessages)
    {
        _userMessages = UserMessages;

        string emailTemplate = Utilities.ReadCustomConfigValue("TravelerProfiler");
        StringBuilder emailTo = new StringBuilder(ConfigurationManager.AppSettings["ToEmail"]);
        StringBuilder emailCC = new StringBuilder(Utilities.ReadCustomConfigValue("ThirdPartyEmailCC"));
        StringBuilder emailFrom = new StringBuilder(Utilities.ReadCustomConfigValue("FromEmail"));
        StringBuilder emailSubject = new StringBuilder(ConfigurationManager.AppSettings["Subject"]);

        formatAndSendThirdPartyEmail(emailTemplate, emailTo,emailCC, emailFrom, emailSubject);
    }

    /// <summary>
    /// Does the actual sending of any email.  Made public so it can be used elsewhere.
    /// </summary>
    /// <param name="toEmail">Required</param>
    /// <param name="ccEmail">NOT Required</param>
    /// <param name="bccEmail">NOT Required</param>
    /// <param name="fromEmail">Required</param>
    /// <param name="subject">Required</param>
    /// <param name="body">Required</param>
    /// <param name="format">Required</param>
    /// <remarks></remarks>
    public void SendEmail(string toEmail,  string bccEmail, string fromEmail, string subject, string body, Enumerations.enumBodyPartFormats format)
    {
        PCentralEmailMessage mEmail = new PCentralEmailMessage();
        PCentralEmailBodyPart eBody = new PCentralEmailBodyPart(body, format);

        mEmail.AddFrom(fromEmail, fromEmail);
        mEmail.Subject = subject;
        mEmail.Priority = Enumerations.enumEmailPriorities.priorityNormal;
        mEmail.BodyParts.Add(eBody);

        string[] toValues = toEmail.Split(new[] { ';' });
        foreach (string email in toValues)
        {
            mEmail.AddRecipient(email);
        }

        //string[] ccValues = ccEmail.Split(new[] { ';' });

        //foreach (string email in ccValues)
        //{
        //    mEmail.AddRecipient(email, "", Enumerations.enumRecipientTypes.recipientCC);
        //}

        if (mEmail.Recipients.Count > 0)
        {
            mEmail.Send();
        }
        else
        {
            throw new Exception("SendEmail: Recipient Address Missing.");
        }
    }

    #endregion

    #region " ### Private Methods ### "

    private string GetCustomEmailPreview(string emailTemplate, StringBuilder emailSubject)
    {
        string participantName = string.Empty;
        string participantEmail = string.Empty;
        string participantUsername = string.Empty;
        string participantPassword = string.Empty;

        getParticipantInfo(ref participantName, ref participantEmail, ref participantUsername, ref participantPassword);

        string programName = string.Empty;
        string programHQName = string.Empty;
        string program800Nbr = string.Empty;
        string programEmail = string.Empty;
        string programURL = string.Empty;

        getProgramInfo(ref programName, ref programHQName, ref program800Nbr, ref programEmail, ref programURL);

        StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

        ////// Replace any constants in the body of the email
        emailBody.Replace(PARTICIPANT_NAME, participantName);
        emailBody.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailBody.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailBody.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailBody.Replace(PROGRAM_NAME, programName);
        emailBody.Replace(PROGRAM_800NBR, program800Nbr);
        emailBody.Replace(PROGRAM_HQNAME, programHQName);
        emailBody.Replace(PROGRAM_URL, programURL);

        ////// Replace any constants in the subject of the email
        emailSubject.Replace(PARTICIPANT_NAME, participantName);
        emailSubject.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailSubject.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailSubject.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailSubject.Replace(PROGRAM_NAME, programName);
        emailSubject.Replace(PROGRAM_800NBR, program800Nbr);
        emailSubject.Replace(PROGRAM_HQNAME, programHQName);
        emailSubject.Replace(PROGRAM_URL, programURL);

        ////// Send the email (HTML)

        string email = "To: " + participantEmail + "<br/>";
        email += "From: " + programEmail + "<br/>";
        email += "Subject: " + emailSubject.ToString() + "<br/>";
        email += "<hr />";
        email += emailBody.ToString();

        return email;
    }

    private void formatAndSendCustomEmail(string emailTemplate, StringBuilder emailSubject)
    {
        string participantName = string.Empty;
        string participantEmail = string.Empty;
        string participantUsername = string.Empty;
        string participantPassword = string.Empty;

        getParticipantInfo(ref participantName, ref participantEmail, ref participantUsername, ref participantPassword);

        string programName = string.Empty;
        string programHQName = string.Empty;
        string program800Nbr = string.Empty;
        string programEmail = string.Empty;
        string programURL = string.Empty;

        getProgramInfo(ref programName, ref programHQName, ref program800Nbr, ref programEmail, ref programURL);

        StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

        ////// Replace any constants in the body of the email
        emailBody.Replace(PARTICIPANT_NAME, participantName);
        emailBody.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailBody.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailBody.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailBody.Replace(PROGRAM_NAME, programName);
        emailBody.Replace(PROGRAM_800NBR, program800Nbr);
        emailBody.Replace(PROGRAM_HQNAME, programHQName);
        emailBody.Replace(PROGRAM_URL, programURL);

        ////// Replace any constants in the subject of the email
        emailSubject.Replace(PARTICIPANT_NAME, participantName);
        emailSubject.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailSubject.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailSubject.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailSubject.Replace(PROGRAM_NAME, programName);
        emailSubject.Replace(PROGRAM_800NBR, program800Nbr);
        emailSubject.Replace(PROGRAM_HQNAME, programHQName);
        emailSubject.Replace(PROGRAM_URL, programURL);

        ////// Send the email (HTML)
        SendEmail(participantEmail,  string.Empty, programEmail, emailSubject.ToString(), emailBody.ToString(), PCentralLib.Enumerations.enumBodyPartFormats.formatHTML);
    }

    private void getProgramInfo(ref string programName, ref string programHQName, ref string program800Nbr, ref string programEmail, ref string programURL)
    {
        PCentralProgram Program = PCentralProgram.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID.ToString());

        programName = Program.ProgramName;
        programHQName = Program.TravelHeadquartersName;
        program800Nbr = Program.ProgramTollFreeNbr;
        programEmail = Program.FromEmailAddress;
        programURL = Program.WebRegistrationSiteUrl;
    }

    private PCentralParticipant getParticipantInfo(ref string participantName, ref string participantEmail, ref string participantUserName, ref string participantPassword)
    {
        PCentralParticipant participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

        if (participant != null)
        {
            participantName = participant.Person.FullName;
            participantEmail = participant.Person.EmailAddresses[0].ToString();
            participantUserName = participant.Person.SignOn.UserName;
            participantPassword = participant.Person.SignOn.Password;
        }
        else
        {
            participantName = string.Empty;
            participantEmail = string.Empty;
            participantUserName = string.Empty;
            participantPassword = string.Empty;
        }

        return participant;
    }

    private void formatAndSendCustomEmail(string emailTemplate, StringBuilder emailSubject, string toEmail, string firstname, string lastname, string password)
    {
        string programName = string.Empty;
        string programHQName = string.Empty;
        string program800Nbr = string.Empty;
        string programEmail = string.Empty;
        string programURL = string.Empty;

        getProgramInfo(ref programName, ref programHQName, ref  program800Nbr, ref  programEmail, ref  programURL);

        StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

        //// Replace any constants in the body of the email
        emailBody.Replace(PARTICIPANT_FIRSTNAME, firstname);
        emailBody.Replace(PARTICIPANT_LASTNAME, lastname);
        emailBody.Replace(PARTICIPANT_PASSWORD, password);
        emailBody.Replace(PROGRAM_NAME, programName);
        emailBody.Replace(PROGRAM_800NBR, program800Nbr);
        emailBody.Replace(PROGRAM_URL, programURL);

        //// Replace any constants in the subject of the email
        emailSubject.Replace(PROGRAM_NAME, programName);
        emailSubject.Replace(PROGRAM_800NBR, program800Nbr);
        emailSubject.Replace(PROGRAM_HQNAME, programHQName);
        emailSubject.Replace(PROGRAM_URL, programURL);

        //// Send the email (HTML)
        SendEmail(toEmail,  string.Empty, programEmail, emailSubject.ToString(), emailBody.ToString(), PCentralLib.Enumerations.enumBodyPartFormats.formatHTML);
    }

    private void formatAndSendThirdPartyEmail(string emailTemplate, StringBuilder emailTo, StringBuilder emailCC, StringBuilder emailFrom, StringBuilder emailSubject)
    {
        string participantName = string.Empty;
        string participantEmail = string.Empty;
        string participantUsername = string.Empty;
        string participantPassword = string.Empty;
        string guestBlock = string.Empty;

        PCentralParticipant participant = getParticipantInfo(ref participantName, ref participantEmail, ref participantUsername, ref participantPassword);

        string programName = string.Empty;
        string programHQName = string.Empty;
        string program800Nbr = string.Empty;
        string programEmail = string.Empty;
        string programURL = string.Empty;

        getProgramInfo(ref programName, ref  programHQName, ref program800Nbr, ref programEmail, ref programURL);

        StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

        // Replace any constants in the body of the email
        emailBody.Replace(PARTICIPANT_NAME, participantName);
        emailBody.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailBody.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailBody.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailBody.Replace(PROGRAM_NAME, programName);
        emailBody.Replace(PROGRAM_800NBR, program800Nbr);
        emailBody.Replace(PROGRAM_HQNAME, programHQName);
        emailBody.Replace(PROGRAM_URL, programURL);

        // Replace any constants in the subject of the email
        emailSubject.Replace(PARTICIPANT_NAME, participantName);
        emailSubject.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailSubject.Replace(PARTICIPANT_USERNAME, participantUsername);
        emailSubject.Replace(PARTICIPANT_PASSWORD, participantPassword);
        emailSubject.Replace(PROGRAM_NAME, programName);
        emailSubject.Replace(PROGRAM_800NBR, program800Nbr);
        emailSubject.Replace(PROGRAM_HQNAME, programHQName);
        emailSubject.Replace(PROGRAM_URL, programURL);

        // Replace any constants in the to/cc/from addresses of the email
        emailTo.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailTo.Replace(PROGRAM_EMAIL, programEmail);
        emailCC.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailCC.Replace(PROGRAM_EMAIL, programEmail);
        emailFrom.Replace(PARTICIPANT_EMAIL, participantEmail);
        emailFrom.Replace(PROGRAM_EMAIL, programEmail);

        // Replace other constants in the third party email
        if (emailBody != null)
        {
            customFieldInfo(ref emailBody);
            profileInfo(ref emailBody, participant);

            ////check for guest
            //if (ContextManager.CurrentGuestParticipantID != 0)
            //{
            //    guestProfileInfo(ref emailBody);
            //}

            LoadPersonContactNumbers(ref emailBody);
        }

        emailBody.Replace(mEmailAddress, participantEmail);
        transportationInfo(ref emailBody);
        replaceGuestBlockGI(ref emailBody);
        replaceGuestBlockTI(ref emailBody);

        // Send the email (PLAIN TEXT)
        SendEmail(emailTo.ToString(),  string.Empty, programEmail, emailSubject.ToString(), emailBody.ToString(), Enumerations.enumBodyPartFormats.formatPlainText);
    }

    // HELPERS
    private void customFieldInfo(ref StringBuilder EmailBody)
    {
        //make sure we found any CF
        if (EmailBody.ToString().Contains(CF_BEGIN))
        {
            Dictionary<string, string> pax_cfs = PCentralProgramCustomField.GetCustomFieldValuesByParticipantID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

            foreach (KeyValuePair<string, string> entry in pax_cfs)
            {
                EmailBody.Replace(string.Concat(CF_BEGIN, entry.Key, CF_END), entry.Value);
            }
        }
    }



    private void replaceGuestBlockGI(ref StringBuilder emailBody)
    {
        dynamic startsAt = emailBody.ToString().IndexOf(GUEST_BLOCK_BEGIN_GI);
        int endsAt = emailBody.ToString().IndexOf(GUEST_BLOCK_END_GI);

        if (startsAt > 0 && endsAt > 0)
        {
            string template = emailBody.ToString().Substring(startsAt + GUEST_BLOCK_BEGIN_GI.Length, endsAt - startsAt - GUEST_BLOCK_END_GI.Length + (GUEST_BLOCK_END_GI.Length - GUEST_BLOCK_BEGIN_GI.Length));

            if (template.Length > 0)
            {
                while (template.EndsWith("\r\n"))
                {
                    template = template.Substring(0, template.Length - 2);
                }

                PCentralParticipant partySummary = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

                if (partySummary.BelongsTo_PartyGuests.Count > 0)
                {
                    StringBuilder guestBlock = new StringBuilder();
                    StringBuilder loopTemplate = new StringBuilder();

                    guestBlock.Length = 0;

                    foreach (PCentralParticipant participant in partySummary.BelongsTo_PartyGuests)
                    {
                        if (participant.PersonID != ContextManager.PersonID)
                        {

                            loopTemplate.Length = 0;
                            loopTemplate.Append(template);
                            loopTemplate.Replace(mGLegalFirstName, participant.Person.FirstName);
                            loopTemplate.Replace(mGLegalLastName, participant.Person.LastName);
                            loopTemplate.Replace(mGLegalMiddleName, participant.Person.MiddleName);
                            loopTemplate.Replace(mGuestDateOfBirth, participant.Person.BirthDate != null ? ((DateTime)participant.Person.BirthDate).ToString(ContextManager.DateFormat) : "");
                            loopTemplate.Replace(mGuestGender, participant.Person.GenderID.ToString());
                            guestBlock.Append(loopTemplate.ToString());
                        }
                    }

                    emailBody.Replace(template, guestBlock.ToString());
                }
                else
                {
                    emailBody.Replace(template, string.Empty);
                }

                emailBody.Replace(GUEST_BLOCK_BEGIN_GI + "\r\n", string.Empty);
                emailBody.Replace(GUEST_BLOCK_END_GI + "\r\n", string.Empty);

            }
        }
    }

    private void replaceGuestBlockTI(ref StringBuilder emailBody)
    {
        int startsAt = emailBody.ToString().IndexOf(GUEST_BLOCK_BEGIN_TI);
        int endsAt = emailBody.ToString().IndexOf(GUEST_BLOCK_END_TI);
        DataSet Airport = PCentralCodeLookupDAL.read_airport_train_stations(Utilities.GetEnvironmentConnection(), null, false, false);

        if (startsAt > 0 && endsAt > 0)
        {
            string template = emailBody.ToString().Substring(startsAt + GUEST_BLOCK_BEGIN_GI.Length, endsAt - startsAt - GUEST_BLOCK_END_TI.Length + (GUEST_BLOCK_END_TI.Length - GUEST_BLOCK_BEGIN_GI.Length));

            if (template.Length > 0)
            {
                while (template.EndsWith("\r\n"))
                {
                    template = template.Substring(0, template.Length - 2);
                }

                PCentralParticipant partySummary = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

                if (partySummary.BelongsTo_PartyGuests.Count > 0 && ContextManager.FeeDisplayTravelType == "ITAAir")
                {
                    PCentralAirPreference ape = null;

                    PCentralEntityList<PCentralAirPreference> AirPreferences = PCentralAirPreference.GetByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

                    StringBuilder guestBlock = new StringBuilder();
                    StringBuilder loopTemplate = new StringBuilder();

                    guestBlock.Length = 0;

                    string FrequentFlyers = string.Empty;

                    foreach (PCentralAirPreference air_prefs in AirPreferences)
                    {
                        if (air_prefs.BelongsTo_Participant.PrimaryFlg == true)
                        {
                            //// Skip the primary participant
                        }
                        else
                        {
                            ape = air_prefs;

                            PCentralTransportationProfile TransProfiles = PCentralTransportationProfile.GetByPersonID(Utilities.GetEnvironmentConnection(), air_prefs.BelongsTo_Participant.PersonID);

                            if (ape != null)
                            {
                                FrequentFlyers = string.Empty;
                                loopTemplate.Length = 0;
                                loopTemplate.Append(template);

                                loopTemplate.Replace(mGPrefHomeDepartureCity, getDisplayValue(ref Airport, "AirportTrainStationID", ape.HomeDepartureCityID));
                                loopTemplate.Replace(mGSeatPreference, TransProfiles.AirlineSeatPreferenceID.toEnumNullable<Enumerations.enumAirSeatPreferences>().ToString());
                                loopTemplate.Replace(mGWheelchair, TransProfiles.AirlineWheelchairRequestID.toEnumNullable<Enumerations.enumAirlineWheelchairRequests>().ToString());
                                loopTemplate.Replace(mGSpecialMeal, TransProfiles.AirlineSpecialMealRequestID.toEnumNullable<Enumerations.enumAirlineSpecialMealRequests>().ToString());

                                foreach (PCentralAirlinePreference airlinePrefEntity in TransProfiles.BelongsTo_Person.AirlinePrefrences)
                                {
                                    if (airlinePrefEntity.FrequentFlyerNbr.Length > 0)
                                    {
                                        FrequentFlyers = string.Concat(FrequentFlyers, "\r\n", "  ", airlinePrefEntity.AirlineID.toEnumNullable<Enumerations.enumAirlineCodes>().ToString() + ": " + airlinePrefEntity.FrequentFlyerNbr.ToString());
                                    }
                                }

                                if (FrequentFlyers.Length == 0)
                                {
                                    FrequentFlyers = "\r\n";
                                }

                                loopTemplate.Replace(mGFrequentFlyerNumber, FrequentFlyers);
                                loopTemplate.Replace(mGLegalFirstName, air_prefs.BelongsTo_Participant.Person.FirstName);
                                loopTemplate.Replace(mGLegalLastName, air_prefs.BelongsTo_Participant.Person.LastName);

                                guestBlock.Append(loopTemplate.ToString());
                            }
                        }
                    }

                    emailBody.Replace(template, guestBlock.ToString());
                }
                else
                {
                    emailBody.Replace(template, string.Empty);
                }
            }
            else
            {
                emailBody.Replace(template, string.Empty);
            }

            emailBody.Replace(GUEST_BLOCK_BEGIN_TI, string.Empty);
            emailBody.Replace(GUEST_BLOCK_END_TI, string.Empty);

        }

    }


    private void profileInfo(ref StringBuilder EmailBody, PCentralParticipant participant)
    {
        if (participant == null)
        {
            participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);
        }

        if (participant != null && (participant.InactiveType == null || participant.InactiveType == default(Enumerations.enumParticipantInactiveTypes)))
        {
            EmailBody.Replace(mLegalFirstName, participant.Person.LegalFirstName);
            EmailBody.Replace(mLegalMiddleName, participant.Person.LegalMiddleName);
            EmailBody.Replace(mLegalLastName, participant.Person.LegalLastName);
            EmailBody.Replace(mDateOfBirth, participant.Person.BirthDate != null ? ((DateTime)participant.Person.BirthDate).ToString(ContextManager.DateFormat) : "");

            switch (participant.Person.GenderID)
            {
                case Enumerations.enumISOGenderCodes.Female:
                    EmailBody.Replace(mGender, "Female");
                    break;
                case Enumerations.enumISOGenderCodes.Male:
                    EmailBody.Replace(mGender, "Male");
                    break;
            }
        }
    }

    private void guestProfileInfo(ref StringBuilder EmailBody, PCentralParticipant gparticipant = null)
    {
        if (gparticipant == null)
        {
            gparticipant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.CurrentGuestParticipantID);
        }

        if (gparticipant != null && (gparticipant.InactiveType == null || gparticipant.InactiveType == default(Enumerations.enumParticipantInactiveTypes)))
        {
            EmailBody.Replace(mGLegalFirstName, gparticipant.Person.LegalFirstName);
            EmailBody.Replace(mGLegalMiddleName, gparticipant.Person.LegalMiddleName);
            EmailBody.Replace(mGLegalLastName, gparticipant.Person.LegalLastName);
            EmailBody.Replace(mGuestDateOfBirth, gparticipant.Person.BirthDate != null ? ((DateTime)gparticipant.Person.BirthDate).ToString(ContextManager.DateFormat) : "");

            switch (gparticipant.Person.GenderID)
            {
                case Enumerations.enumISOGenderCodes.Female:
                    EmailBody.Replace(mGuestGender, "Female");
                    break;
                case Enumerations.enumISOGenderCodes.Male:
                    EmailBody.Replace(mGuestGender, "Male");
                    break;
            }
        }
    }

    private void LoadPersonContactNumbers(ref StringBuilder EmailBody)
    {
        string phone_nbr = string.Empty;

        PCentralEntityList<PCentralPersonContactNumber> numbers = PCentralPersonContactNumber.GetByPersonID(Utilities.GetEnvironmentConnection(), ContextManager.PersonID);

        if (numbers != null && numbers.Count > 0)
        {
            foreach (PCentralPersonContactNumber number in numbers)
            {
                phone_nbr = string.IsNullOrWhiteSpace(number.ContactNbr) ? string.Empty : number.ContactNbr.Trim();

                switch (number.TypeID.toInt())
                {
                    case (int)Enumerations.enumPersonContactNumberTypes.HomePhone:
                        EmailBody.Replace(mHomePhone, phone_nbr);
                        break;
                    case (int)Enumerations.enumPersonContactNumberTypes.BusinessPhone:
                        EmailBody.Replace(mBusinessPhone, phone_nbr + number.Extension.toStringOr(prepend_if_not_empty: " Ext. :"));
                        break;
                    case (int)Enumerations.enumPersonContactNumberTypes.BusinessFax:
                        EmailBody.Replace(mBusinessFax, phone_nbr);
                        break;
                    case (int)Enumerations.enumPersonContactNumberTypes.MobilePhone:
                        EmailBody.Replace(mMobilePhone, phone_nbr);
                        break;
                    default:
                        break; // has no type - for now, do nothing
                }
            }
        }
    }


    private void transportationInfo(ref StringBuilder emailBody)
    {

        PCentralAirPreference ape = null;
        PCentralCodeLookup depart_times = PCentralCodeLookupFunctions.get_code_lookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.PreferredDepartureTimes);
        DataSet Airport = PCentralCodeLookupDAL.read_airport_train_stations(Utilities.GetEnvironmentConnection(), null, false, false);
        PCentralTransportationProfile TransProfiles = PCentralTransportationProfile.GetByPersonID(Utilities.GetEnvironmentConnection(), ContextManager.PersonID);

        PCentralParty Party = PCentralParty.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        string FrequentFlyers = null;
        //'REMOVE ANY NON GROUP HOTEL REQUESTS IF THEY EXIST
        PCentralEntityList<PCentralAirPreference> AirPreferences = PCentralAirPreference.GetByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        foreach (PCentralAirPreference prefs in AirPreferences)
        {
            if (prefs.BelongsTo_Participant.PrimaryFlg == true)
            {
                ape = prefs;
                break;
            }
        }

        if (ContextManager.FeeDisplayTravelType == "Driving")
        {
            emailBody.Replace(mTransportationType, "Driving");
            emailBody.Replace(mTravelDates, string.Concat(ContextManager.TravelDateBegin.ToShortDateString(), "-", ContextManager.TravelDateEnd.ToShortDateString()));
            emailBody.Replace(mPrefHomeDepartureTime, string.Empty);
            emailBody.Replace(mPrefDestDepartureTime, string.Empty);
            emailBody.Replace(mPrefAirline, string.Empty);
            emailBody.Replace(mPrefHomeDepartureCity, string.Empty);
            emailBody.Replace(mWheelchair, string.Empty);
            emailBody.Replace(mSpecialMeal, string.Empty);
            emailBody.Replace(mDriveTime, string.Empty);
            emailBody.Replace(mFrequentFlyerNumber, string.Empty);
            emailBody.Replace(mAirRemarks, string.Empty);
            emailBody.Replace(mSeatPreference, string.Empty);

        }
        else if (ContextManager.FeeDisplayTravelType == "ITAAir")
        {
            emailBody.Replace(mTransportationType, "Air");
            emailBody.Replace(mTravelDates, string.Concat(ape.TravelBeginDT.ToShortDateString(), "-", ape.TravelEndDT.ToShortDateString()));
            emailBody.Replace(mPrefHomeDepartureTime, ape.HomeDepartureTimeID.toEnumNullable<Enumerations.enumPreferredDepartureTimes>().ToString());
            emailBody.Replace(mPrefDestDepartureTime, ape.DestDepartureTimeID.toEnumNullable<Enumerations.enumPreferredDepartureTimes>().ToString()); ;
            emailBody.Replace(mPrefAirline, string.Empty);
            emailBody.Replace(mPrefHomeDepartureCity, getDisplayValue(ref Airport, "AirportTrainStationID", ape.HomeDepartureCityID));
            emailBody.Replace(mDriveTime, ape.DriveTimeToAirport.ToString());
            FrequentFlyers = string.Empty;

            if (TransProfiles != null)

                emailBody.Replace(mSeatPreference, TransProfiles.AirlineSeatPreferenceID.toEnumNullable<Enumerations.enumAirSeatPreferences>().ToString());
            emailBody.Replace(mWheelchair, TransProfiles.AirlineWheelchairRequestID.toEnumNullable<Enumerations.enumAirlineWheelchairRequests>().ToString());
            emailBody.Replace(mSpecialMeal, TransProfiles.AirlineSpecialMealRequestID.toEnumNullable<Enumerations.enumAirlineSpecialMealRequests>().ToString());
            foreach (PCentralAirlinePreference airlinePrefEntity in TransProfiles.BelongsTo_Person.AirlinePrefrences)
            {
                if (airlinePrefEntity.FrequentFlyerNbr.Length > 0)
                {
                    FrequentFlyers = string.Concat(FrequentFlyers, "\r\n", "  ", airlinePrefEntity.AirlineID.toEnumNullable<Enumerations.enumAirlineCodes>().ToString() + ": " + airlinePrefEntity.FrequentFlyerNbr.ToString());
                }
            }

            emailBody.Replace(mFrequentFlyerNumber, FrequentFlyers);
            emailBody.Replace(mAirRemarks, Party.AirComments);

        }
        else if (ContextManager.FeeDisplayTravelType == "OwnAir")
        {
            emailBody.Replace(mTransportationType, "Own Air");
            emailBody.Replace(mTravelDates, string.Concat(ContextManager.TravelDateBegin.ToShortDateString(), "-", ContextManager.TravelDateEnd.ToShortDateString()));
            emailBody.Replace(mPrefHomeDepartureTime, string.Empty);
            emailBody.Replace(mPrefDestDepartureTime, string.Empty);
            emailBody.Replace(mPrefAirline, string.Empty);
            emailBody.Replace(mPrefHomeDepartureCity, string.Empty);
            emailBody.Replace(mWheelchair, string.Empty);
            emailBody.Replace(mSpecialMeal, string.Empty);
            emailBody.Replace(mDriveTime, string.Empty);
            emailBody.Replace(mSeatPreference, string.Empty);
            emailBody.Replace(mFrequentFlyerNumber, string.Empty);
            emailBody.Replace(mAirRemarks, string.Empty);
        }
        //Hotel Nights
        emailBody.Replace(mCheckInDate, ContextManager.GroupHotelDateBegin.ToString());
        emailBody.Replace(mCheckOutDate, ContextManager.GroupHotelDateEnd.ToString());

    }



    private string getDisplayValue(ref DataSet ds, string filterName, Int32 id)
    {
        foreach (DataRow dr in ds.Tables[0].Select(string.Concat(filterName, " = ", id.ToString())))
        {
            return dr["DisplayValue"].ToString();
        }
        return string.Empty;
    }


    #endregion
}
