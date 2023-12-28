#region " ### Imports ### "

using PCentralLib.participants;
using PCentralLib.person;
using System.Data;
using System;
using PCentralLib;
using System.Collections;
using System.Text;
using PCentralLib.programs;
using ITALib;
using ITALib.DataAccess;
using System.Collections.Generic;
using System.Linq;

#endregion

#region " ### View ### "
public enum enumTriBoolean
{
    tbUnassigned = 0,
    tbTrue = 1,
    tbFalse = 2
}

public interface IGuestProfileView : IPageViewBase
{
    DataTable PrefixList { set; }
    DataTable SuffixList { set; }
    DataTable GenderList { set; }
    DataTable CountryList { set; }

    DataTable ProofofCitizenshipTypeList { set; }
    //'GENERAL INFO
    string LegalName { get; set; }
    string Nickname { get; set; }
    string BadgeFirstName { get; set; }
    string BadgeLastName { get; set; }
    //'PERSONAL INFO
    string DateOfBirth { get; set; }
    string GetSetGender { get; set; }
    //'Proof of Citizenship
    bool MoreThanOneCitizenshipTypeAvailable { get; set; }
    string CitizenshipTypeIfOnlyOneAvailable { get; set; }

    Enumerations.enumProofofCitizenshipTypes GetSetProofofCitizenshipType { get; set; }
    //'Passport Details
    //Property PassportStatusAvailable() As Boolean
    string FirstPassportName { get; set; }
    string MiddlePassportName { get; set; }
    string LastPassportName { get; set; }
    string GetSetSuffixPassport { get; set; }
    string PassportNumber { get; set; }
    string PassportExpirationDate { get; set; }

    string PassportIssueDate { get; set; }

    string PassportIssuingAuthority { get; set; }
    string GetSetNationality { get; set; }
    string GetSetCountryOfIssue { get; set; }
    //'Birth Certificate Details:
    string GetSetBirthCertificate { get; set; }
    //'Special Requests:
    string DietaryPreferences { get; set; }
    //'Physical Assistance Requests:
    string PhysicalAssistanceRequests { get; set; }
    //Property IsPassportAvailableSelected() As Boolean
    enumTriBoolean HasPassportInformation { get; set; }
    //'Subs to set up multiple properties for multiple controls
    string LegalPrefix { get; set; }
    string LegalFirstName { get; set; }
    string LegalMiddleName { get; set; }
    string LegalLastName { get; set; }
    string LegalSuffix { get; set; }
    //page setting properties
    string CurrentPage { get; }
    bool WelcomeTextVisible { set; }
    string WelcomeText { set; }
    bool InformationTextVisible { set; }
    string InformationText { set; }
    bool GeneralInfoSectionVisible { get; set; }
    string GeneralInfoSectionHeadingText { set; }
    bool NameSubSectionVisible { get; set; }
    string NameSubSectionText { set; }
    bool DisplayNameVisible { get; set; }
    string DisplayNameText { set; }
    bool EditableLegalNameVisible { get; set; }
    string PrefixText { set; }
    bool PrefixVisible { get; set; }
    string SuffixText { set; }
    bool SuffixVisible { get; set; }
    //
    bool ValidatePassportName { get; set; }
    //
    bool NickNameVisible { get; set; }
    string NickNameText { get; set; }
    bool BadgeNameVisible { get; set; }
    bool BadgeFNameRequired { get; set; }
    bool BadgeLNameRequired { get; set; }
    string BadgeFNameText { set; }
    string BadgeLNameText { set; }
    string PersonalInfoHeaderText { set; }
    bool PersonalInfoSectionVisible { get; set; }
    bool DOBVisible { get; set; }
    bool DOBRequired { get; set; }
    string DOBText { set; }
    bool GenderVisible { get; set; }
    bool GenderRequired { get; set; }
    string GenderText { set; }
    //POC
    bool ProofOfCitizenshipVisible { get; set; }
    string POCSectionHeader { set; }
    string POCSubSection { set; }

    bool POCRequired { get; set; }
    bool POCTypePassportOnly { get; set; }

    string POCTypeText { set; }
    bool PassportVisible { get; set; }
    string PassportSubSection { set; }
    string PassportAvailableText { set; }
    string PassportStatusYesText { set; }
    string PassportStatusNoText { set; }
    string PassportFNameText { set; }
    string PassportMNameText { set; }
    string PassportLNameText { set; }
    string PassportSuffixText { set; }
    string PassportNbrText { set; }
    string PassportExpDateText { set; }

    string PassportIssueDateText { set; }
    string PassportIssueAuthorityText { set; }
    string PassportNationText { set; }
    string PassportIssuedText { set; }
    bool BirthCertificateVisible { get; set; }
    string BirthCSubSection { set; }
    string BCertNationText { set; }
    //special requests
    bool SpecialRequestVisible { get; set; }
    string SpecialReqSubSection { set; }
    bool SpecialReqSubSectionVisible { set; }
    string DietPrefText { set; }
    string PhysicalReqText { set; }
    //custom fields
    string ProfileCustomFields { get; set; }

    string ProfileCustomFieldsRequired { get; set; }

    string LegalNameVerbiage { get; set; }

    bool LegalNameVerbiageVisible { get; set; }
    void AddCustomControl();
}

#endregion

#region " ### Presenter ### "

public class GuestProfilePresenter : PresenterBase
{
    private GuestProfileInformationDTO settingsDTO = new GuestProfileInformationDTO();

    #region " ### Constructor Code ### "

    public GuestProfilePresenter(IGuestProfileView myView)
    {
        View = myView;
    }

    public IGuestProfileView View
    {
        get { return (IGuestProfileView)_View; }
        set { base._View = value; }
    }

    #endregion

    #region " ### Public Presenter Methods ### "

    public void PreInit()
    {
        //// this is in the PresenterBase...
        SetCustomFieldParticipantIDToCurrentGuestParticipantID();
    }

    public void Init()
    {
        PCentralParticipant participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.CurrentGuestParticipantID);

        if (Utilities.ShouldReadWebRegSettings(View.CurrentPage))
        {
            SetUpPage(); // add custom fields from db to the form
            View.AddCustomControl(); // this is needed for guest.
            SetCustomFieldParticipantIDToCurrentGuestParticipantID();
        }

        loadData(participant);
        LoadCustomFieldControls();
    }

    /// <summary>
    /// Reads Page setting from DB set up by user thru WebRegAdmin.
    /// Sets up page controls based on data from database.
    /// </summary>
    /// <remarks></remarks>
    public void SetUpPage()
    {
        PageConfiguration pageSettings = new PageConfiguration();
        DataSet settingsDS = pageSettings.ReadPageConfigSettings(View.CurrentPage);
        this.settingsDTO = new GuestProfileInformationDTO(settingsDS);

        if (settingsDS.HasTables())
        {
            View.LegalNameVerbiageVisible = false;
            //check if we need to show the page
            if (settingsDS.HasRows() && (!settingsDS.Tables[0].Rows[0]["ItemValue"].toBool()))
            {
                //page was marked as SKIP, move to next page
                Utilities.Navigate(Utilities.NextPage());
            }

            View.LegalNameVerbiageVisible = settingsDTO.LblLegalNameVerbiageVisible;
            if (settingsDTO.LblLegalNameVerbiageVisible)
            {

                if (!String.IsNullOrWhiteSpace(settingsDTO.LblLegalNameVerbiageText))
                {
                    View.LegalNameVerbiage = settingsDTO.LblLegalNameVerbiageText;
                }
            }

            //set up page here
            View.WelcomeTextVisible = settingsDTO.WelcometextVisible;
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.WelcomeText))
            {
                View.WelcomeText = settingsDTO.WelcomeText;
            }
            View.InformationTextVisible = settingsDTO.InformationTextVisible;
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Informationtext))
            {
                View.InformationText = settingsDTO.Informationtext;
            }
            View.GeneralInfoSectionVisible = settingsDTO.GeneralInfoSectionVisible;
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.GeneralInfoSectionHeadingText))
            {
                View.GeneralInfoSectionHeadingText = settingsDTO.GeneralInfoSectionHeadingText;
            }
            View.NameSubSectionVisible = settingsDTO.NameSubSectionVisible;
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.NameSubSectionText))
            {
                View.NameSubSectionText = settingsDTO.NameSubSectionText;
            }
            View.DisplayNameVisible = settingsDTO.DisplayNameVisible;
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.DisplayNameText))
            {
                View.DisplayNameText = settingsDTO.DisplayNameText;
            }
            View.EditableLegalNameVisible = settingsDTO.EditableLegalNameVisible;
            if (settingsDTO.EditableLegalNameVisible)
            {
                View.PrefixVisible = settingsDTO.Prefixvisible;
                if (settingsDTO.Prefixvisible)
                {
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Prefixtext))
                    {
                        View.PrefixText = settingsDTO.Prefixtext;
                    }
                }
                View.SuffixVisible = settingsDTO.Suffixvisible;
                if (settingsDTO.Suffixvisible)
                {
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Suffixtext))
                    {
                        View.SuffixText = settingsDTO.Suffixtext;
                    }
                }
            }
            View.NickNameVisible = settingsDTO.NicknameVisible;
            if (settingsDTO.NicknameVisible)
            {
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.NickNameText))
                {
                    View.NickNameText = settingsDTO.NickNameText;
                }
            }
            View.BadgeNameVisible = settingsDTO.BadgeNameVisible;
            if (settingsDTO.BadgeNameVisible)
            {
                View.BadgeFNameRequired = settingsDTO.RequireBadgeFName;
                View.BadgeLNameRequired = settingsDTO.RequireBadgeLName;
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.BadgeFNameText))
                {
                    View.BadgeFNameText = settingsDTO.BadgeFNameText;
                }
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.BadgeLNameText))
                {
                    View.BadgeLNameText = settingsDTO.BadgeLNameText;
                }
            }
            View.DOBVisible = settingsDTO.DOBVisible;
            if (settingsDTO.DOBVisible)
            {
                View.DOBRequired = settingsDTO.DOBRequire;
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.DOBtext))
                {
                    View.DOBText = settingsDTO.DOBtext;
                }
            }
            View.GenderVisible = settingsDTO.GenderVisible;
            if (settingsDTO.GenderVisible)
            {
                View.GenderRequired = settingsDTO.GenderRequire;
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Gendertext))
                {
                    View.GenderText = settingsDTO.Gendertext;
                }
            }
            if (View.DOBVisible == false && View.GenderVisible == false)
            {
                View.PersonalInfoSectionVisible = false;
            }
            View.PersonalInfoSectionVisible = settingsDTO.ProfileHeadervisible;
            if (View.PersonalInfoSectionVisible)
            {
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.ProfileHeadertext))
                {
                    View.PersonalInfoHeaderText = settingsDTO.ProfileHeadertext;
                }
            }

            //POC
            if (settingsDTO.POCVisible)
            {
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.POCSectionHeader))
                {
                    View.POCSectionHeader = settingsDTO.POCSectionHeader;
                }
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.POCSubSection))
                {
                    View.POCSubSection = settingsDTO.POCSubSection;
                }
                View.POCRequired = settingsDTO.POCRequired;
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.POCTypeText))
                {
                    View.POCTypeText = settingsDTO.POCTypeText;
                }
                View.POCTypePassportOnly = settingsDTO.POCTypePassportOnly;

                //If Not Utilities.IsNothingNullOrEmpty(settingsDTO.POCCountryText) And settingsDTO.POCCountryofBirthVisible Then
                //    View.POCCountryText = settingsDTO.POCCountryText
                //End If
            }
            else
            {
                View.ProofOfCitizenshipVisible = settingsDTO.POCVisible;
            }

            //Passport
            if (settingsDTO.POCVisible)
            {
                if (settingsDTO.PassportVisible)
                {
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportSubSection))
                    {
                        View.PassportSubSection = settingsDTO.PassportSubSection;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportAvailableText))
                    {
                        View.PassportAvailableText = settingsDTO.PassportAvailableText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportStatusYesText))
                    {
                        View.PassportStatusYesText = settingsDTO.PassportStatusYesText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportStatusNoText))
                    {
                        View.PassportStatusNoText = settingsDTO.PassportStatusNoText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportFNameText))
                    {
                        View.PassportFNameText = settingsDTO.PassportFNameText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportFNameText))
                    {
                        View.PassportFNameText = settingsDTO.PassportFNameText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportMNameText))
                    {
                        View.PassportMNameText = settingsDTO.PassportMNameText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportLNameText))
                    {
                        View.PassportLNameText = settingsDTO.PassportLNameText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportSuffixText))
                    {
                        View.PassportSuffixText = settingsDTO.PassportSuffixText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportNbrText))
                    {
                        View.PassportNbrText = settingsDTO.PassportNbrText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Passportissuedatetext))
                    {
                        View.PassportIssueDateText = settingsDTO.Passportissuedatetext;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Passportcityissuetext))
                    {
                        //  View.PassportCityIssueText = settingsDTO.Passportcityissuetext
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Passportissueauthoritytext))
                    {
                        View.PassportIssueAuthorityText = settingsDTO.Passportissueauthoritytext;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportExpDateText))
                    {
                        View.PassportExpDateText = settingsDTO.PassportExpDateText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportNationText))
                    {
                        View.PassportNationText = settingsDTO.PassportNationText;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PassportIssuedText))
                    {
                        View.PassportIssuedText = settingsDTO.PassportIssuedText;
                    }
                }
                else
                {
                    View.PassportVisible = settingsDTO.PassportVisible;
                }
            }

            //Birth Cert
            if (settingsDTO.POCVisible)
            {
                if (settingsDTO.BirthCertificateVisible)
                {
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.BirthCSubSection))
                    {
                        View.BirthCSubSection = settingsDTO.BirthCSubSection;
                    }
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.BCertNationText))
                    {
                        View.BCertNationText = settingsDTO.BCertNationText;
                    }
                }
                else
                {
                    View.BirthCertificateVisible = settingsDTO.BirthCertificateVisible;
                }
            }

            //Special Request
            if (settingsDTO.SpecialRequestVisible)
            {
                View.SpecialReqSubSectionVisible = settingsDTO.SpecialReqSubSectionVisible;
                if (settingsDTO.SpecialReqSubSectionVisible)
                {
                    if (!Utilities.IsNothingNullOrEmpty(settingsDTO.SpecialReqSubSection))
                    {
                        View.SpecialReqSubSection = settingsDTO.SpecialReqSubSection;
                    }
                }
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.DietPrefText))
                {
                    View.DietPrefText = settingsDTO.DietPrefText;
                }
                if (!Utilities.IsNothingNullOrEmpty(settingsDTO.PhysicalReqText))
                {
                    View.PhysicalReqText = settingsDTO.PhysicalReqText;
                }
            }
            else
            {
                View.SpecialRequestVisible = settingsDTO.SpecialRequestVisible;
            }

            //custom fields
            if (!Utilities.IsNothingNullOrEmpty(settingsDTO.Profilecustomfields))
            {
                //this collection has names of custom fields to be created on the form
                View.ProfileCustomFields = settingsDTO.Profilecustomfields;

                //this collection has true/false flag if CF from above collection is required
                View.ProfileCustomFieldsRequired = settingsDTO.Profilecustomfieldsrequired;

                // If you have a custom logic that needs to remove or switch if CF is required
                // Call the following method in Utilities file.
                /*
                if (ContextManager.LoginForSelfEnrollPassword.ToLower() != "some CuSTom LoGiC")
                {
                    //custom for this program
                    //Change if CF is required on not by creating collection of items
                    SortedList<string, string> changestatusCollection = new SortedList<string, string>();
                    changestatusCollection.Add("CustomFieldName", "false");
                    //custom field needs to be changed from required to not required
                    //remove any custom fields passed to the method
                    Utilities.GetCustomfieldNameandStatus(View.ProfileCustomFields, View.ProfileCustomFieldsRequired, "Title", changestatusCollection);
                }
                //*/
            }
        }
    }

    private void ValidateInput(ref PCentralValidationResults validationlist)
    {
        //// Legal First Name and Last Name are required
        Utilities.CheckRequired(View.LegalFirstName, "Legal First Name", ref validationlist);
        Utilities.CheckRequired(View.LegalLastName, "Legal Last Name", ref validationlist);

        if (View.DOBVisible)
        {
            if (View.DOBRequired && View.DateOfBirth.isEmpty())
            {
                Utilities.CheckRequired("", "Date of Birth", ref validationlist);
            }
            else if (View.DateOfBirth.isNotEmpty())
            {
                if (!View.DateOfBirth.isDateTime())
                {
                    validationlist.add_error("Please enter a valid Date of Birth", false);
                }
                else if (View.DateOfBirth.toDateTime().occursOnOrAfter(DateTime.Today))
                {
                    validationlist.add_error("Date of Birth must be less than today's date", false);
                }
            }
        }

        if (View.GenderVisible && Convert.ToInt32(View.GetSetGender) == 0)
        {
            Utilities.CheckRequired(Convert.ToInt32(View.GetSetGender), "Gender", ref validationlist);
        }

        if (View.PassportVisible && (View.POCTypePassportOnly || View.GetSetProofofCitizenshipType == Enumerations.enumProofofCitizenshipTypes.Passport) && View.HasPassportInformation == enumTriBoolean.tbUnassigned)
        {
            Utilities.CheckRequired("", "Passport Status", ref validationlist);
        }
    }

    public void Save()
    {
        View.UserMessages.Text = string.Empty;

        DataSet NameSuffixes = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.NameSuffixes);

        PCentralValidationResults validation = new PCentralValidationResults();

        PCentralParticipant participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.CurrentGuestParticipantID);
        GetParticipant(ref participant, ref validation);

        PCentralPerson person = participant.Person;
        GetPerson(ref person, ref validation);

        PCentralProofOfCitizanship poc = participant.Person.ProofOfCitizenship;
        GetPOC(ref participant, ref person, ref  NameSuffixes, ref poc, ref validation);

        ValidatePassportInfo(ref validation);

        PCentralPersonProfile profile = participant.Person.PersonProfile;
        GetProfileInfo(ref person, ref profile, ref validation);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///SAVE

        SetCustomFieldControls(participant, ref validation, save: false);

        if (validation.has_errors_or_warnings)
        {
            View.UserMessages.CssClass = "warning";
            View.UserMessages.Text += Utilities.DisplayAllErrorsAndWarningsbr(validation);
            return;
        }

        PCentralSaveResults saveresults = participant.Save(Guid.NewGuid(), ContextManager.RealUserID, false);

        // check for success or failure
        if (!saveresults.was_successful)
        {
            View.UserMessages.CssClass = "warning";
            View.UserMessages.Text = saveresults.validation_results.get_all_errors_br(); // this has many errors lists in any way you like it
        }
        else if (ContextManager.AllowedNumberOfGuests <= 1)
        {
            if (ContextManager.IsRegistered)
            {
                Utilities.Navigate(Utilities.NextPage("single_guest_registered"));
            }
            else
            {
                Utilities.Navigate(Utilities.NextPage("single_guest_unregistered"));
            }
        }
        else
        {
            Utilities.Navigate(Utilities.NextPage("multiple_guests"));
        }
    }

    #endregion

    #region " ### Private Methods ### "

    private void GetProfileInfo(ref PCentralPerson person, ref PCentralPersonProfile profile, ref PCentralValidationResults validation)
    {
        if (profile == null) profile = new PCentralPersonProfile(Utilities.GetEnvironmentConnection()) { PersonID = person.PersonID };

        //check required fields
        if (View.BadgeNameVisible)
        {
            if (View.BadgeFNameRequired)
            {
                Utilities.CheckRequired(View.BadgeFirstName, "Badge First Name", ref validation);
            }
            if (View.BadgeLNameRequired)
            {
                Utilities.CheckRequired(View.BadgeLastName, "Badge Last Name", ref validation);
            }

            profile.BadgeFirstName = View.BadgeFirstName;
            profile.BadgeLastName = View.BadgeLastName;
        }

        profile.DietaryPreferences = View.DietaryPreferences.Replace("\r\n", "  ");
        profile.PhysicalAssistance = View.PhysicalAssistanceRequests.Replace("\r\n", "  ");

        if (View.BadgeNameVisible)
        {
            // character validation
            validation.combine_with(profile.PerformCharacterValidation(
                badge_name_required: false,
                award_name_required: false,
                emergency_contact_required: false,
                vip_title_required: false,
                validate_br127: false));
        }

        person.PersonProfile = profile;
        person.PersonProfile.TitleCaseFields();


    }

    private void ValidatePassportInfo(ref PCentralValidationResults validation)
    {
        bool data_provided = false;
        string error_text = "You have provided your passport information but selected \"No, I will provide it later.\"  Please select \"Yes\" or remove your passport information.";

        Action<int> check_input = (int i) => { data_provided = (data_provided || i > 0); };

        if (View.HasPassportInformation == enumTriBoolean.tbFalse && View.PassportVisible)
        {
            check_input(View.PassportNumber.Length);
            check_input(View.FirstPassportName.Length);
            check_input(View.MiddlePassportName.Length);
            check_input(View.LastPassportName.Length);
            check_input(View.GetSetSuffixPassport.toInt());
            check_input(View.PassportExpirationDate.Length);
            check_input(View.PassportIssueDate.Length);
            check_input(View.GetSetCountryOfIssue.toInt());
            check_input(View.GetSetNationality.toInt());
            check_input(View.PassportIssuingAuthority.Length);

            if (data_provided)
            {
                validation.add_error(error_text, false);
            }
        }
    }

    private void GetPOC(ref PCentralParticipant Participant, ref PCentralPerson Person, ref DataSet dsSuffixes, ref PCentralProofOfCitizanship poc, ref PCentralValidationResults validation)
    {
        DateTime defaultEndDate;
        string person_name;
        string passport_name;

        if (poc == null) poc = new PCentralProofOfCitizanship(Utilities.GetEnvironmentConnection());

        if (View.ProofOfCitizenshipVisible && (!View.POCTypePassportOnly) && Convert.ToInt32(View.GetSetProofofCitizenshipType) == 0)
        {
            Utilities.CheckRequired(0, "Proof of Citizenship Type", ref validation);
        }

        //if (View.PassportVisible && View.GetSetProofofCitizenshipType.toEnumNullable<Enumerations.enumProofofCitizenshipTypes>() == Enumerations.enumProofofCitizenshipTypes.Passport)
        if (View.PassportVisible)
        {
            if (View.HasPassportInformation == enumTriBoolean.tbUnassigned)
            {
                Utilities.CheckRequired("", "Passport Status", ref validation);
            }

            int suffixPassport;
            int.TryParse(View.GetSetSuffixPassport, out suffixPassport);
            if (suffixPassport > 0)
            {
                poc.PassportSuffixID = (Enumerations.enumNameSuffixes)(suffixPassport);
            }

            int countryOfIssue;
            int.TryParse(View.GetSetCountryOfIssue, out countryOfIssue);
            if (countryOfIssue > 0)
            {
                poc.PassportCountryOfIssueID = (Enumerations.enumISOCountryCodes)(countryOfIssue);
            }

            int nationality;
            int.TryParse(View.GetSetNationality, out nationality);
            if (nationality > 0)
            {
                poc.PassportNationalityID = (Enumerations.enumISOCountryCodes)(nationality);
            }

            // poc.CityOfIssue = View.PassportCityOfIssue
            poc.PassportFirstName = View.FirstPassportName;
            poc.PassportMiddleName = View.MiddlePassportName;
            poc.PassportLastName = View.LastPassportName;
            poc.PassportNumber = View.PassportNumber;
            poc.IssuingAuthority = View.PassportIssuingAuthority;
            //poc.PassportIssueDate = View.PassportIssueDate.toDateTimeNullable();
            //poc.PassportExpirationDate = View.PassportExpirationDate.toDateTimeNullable();
            if (!Utilities.IsNothingNullOrEmpty(View.PassportIssueDate))
            {
                poc.PassportIssueDate = Convert.ToDateTime(View.PassportIssueDate);
            }
            if (!Utilities.IsNothingNullOrEmpty(View.PassportExpirationDate))
            {
                poc.PassportExpirationDate = Convert.ToDateTime(View.PassportExpirationDate);
            }

            //Web Reg Required fields
            if (Participant.PassportStatus == Enumerations.enumPassportStatuses.Available)
            {
                // get default program end date to validate passport
                defaultEndDate = Participant.BelongsTo_Program.ProgramTravelBeginDT;

                Utilities.CheckRequired(poc.PassportFirstName, "Passport First Name", ref validation);
                Utilities.CheckRequired(poc.PassportLastName, "Passport Last Name", ref validation);
                Utilities.CheckRequired(poc.PassportNumber, "Passport Number", ref  validation);
                Utilities.CheckRequired(Convert.ToInt32(poc.PassportCountryOfIssueID), "Passport Country Of Issue", ref validation);

                // Validate Dates...

                if (View.PassportExpirationDate.Length == 0)
                {
                    Utilities.CheckRequired(string.Empty, "Passport Expiration Date", ref validation);
                }
                else if (View.PassportExpirationDate.isDateTime())
                {
                    View.PassportExpirationDate = View.PassportExpirationDate.toDateTime().ToString(ContextManager.DateFormat);

                    if (View.PassportExpirationDate.toDateTime() <= defaultEndDate.AddMonths(6))
                    {
                        validation.add_error("Your passport must be valid for at least six months following the last date of your trip.", false);
                    }
                }
                else
                {
                    validation.add_error("Please enter a valid Passport Expiration Date", false);
                }

                if (View.PassportIssueDate.Length == 0)
                {
                    Utilities.CheckRequired(string.Empty, "Passport Issue Date", ref validation);
                }
                else if (View.PassportIssueDate.isDateTime())
                {
                    View.PassportIssueDate = View.PassportIssueDate.toDateTime().ToString(ContextManager.DateFormat);

                    if (View.PassportExpirationDate.isDateTime() && View.PassportIssueDate.toDateTime() > View.PassportExpirationDate.toDateTime())
                    {
                        validation.add_error("Your Passport Issue Date can not be greater than passport Expiration Date.", false);
                    }
                    if (View.PassportIssueDate.toDateTime(DateTime.MinValue) > DateTime.Today)
                    {
                        validation.add_error("Your Passport Issue Date can not be greater than today's date.", false);
                    }
                }
                else
                {
                    validation.add_error("Please enter a valid Passport Issue Date", false);
                }

                Utilities.CheckRequired(poc.IssuingAuthority, "Passport Issuing Authority", ref validation);
                Utilities.CheckRequired(Convert.ToInt32((!String.IsNullOrWhiteSpace(View.GetSetNationality) ? View.GetSetNationality : "0")), "Passport Nationality", ref validation);

                // ensure that passport name and person names match
                //if (Utilities.ReadCustomConfigValue("pages/" + View.CurrentPage + ".aspx", "option_passport_name_validation").toBool())
                if (settingsDTO.ValidatePassportName)
                {
                    person_name = PCentralPerson.ConcatinateName(null, Person.FirstName, Person.MiddleName, Person.LastName, null);
                    passport_name = PCentralPerson.ConcatinateName(null, View.FirstPassportName, View.MiddlePassportName, View.LastPassportName, null);

                    if (string.Compare(person_name, passport_name, true) != 0)
                    {
                        validation.add_warning(Utilities.ReadMessageConfigValue("rules/Profile_PassportNameLegalNameNotSame"));
                    }
                }
            }
        }

        if (View.PassportVisible)
        {
            // character validation
            validation.combine_with(poc.PerformCharacterValidation(
                passport_name_required: false,
                passport_number_required: false,
                city_of_issue_required: false,
                issuing_auth_required: false));

            // title case
            poc.TitleCaseFields();
        }

        Person.ProofOfCitizenship = poc;
    }

    private void GetPerson(ref PCentralPerson person, ref PCentralValidationResults validation)
    {
        if (View.EditableLegalNameVisible)
        {
            Utilities.CheckRequired(View.LegalFirstName, "Legal First Name", ref validation);
            Utilities.CheckRequired(View.LegalLastName, "Legal Last Name", ref validation);

            person.LegalFirstName = View.LegalFirstName;
            person.LegalMiddleName = View.LegalMiddleName;
            person.LegalLastName = View.LegalLastName;
            int prefix;
            int.TryParse(View.LegalPrefix, out prefix);
            person.LegalPrefix = (Enumerations.enumNamePrefixes)prefix;
            int suffix;
            int.TryParse(View.LegalSuffix, out suffix);
            person.LegalSuffix = (Enumerations.enumNameSuffixes)suffix;
        }

        if (View.GenderVisible)
        {
            int gender;
            int.TryParse(View.GetSetGender, out gender);
            person.GenderID = (Enumerations.enumISOGenderCodes)gender;

            if (person.GenderID == Enumerations.enumISOGenderCodes.NotSpecified)
            {
                validation.add_required_field_error("Gender");
            }
        }

        if (View.DOBVisible)
        {
            if (View.DOBRequired && View.DateOfBirth.isEmpty())
            {
                Utilities.CheckRequired("", "Date of Birth", ref validation);
            }
            else if (View.DateOfBirth.isNotEmpty())
            {
                if (!View.DateOfBirth.isDateTime())
                {
                    validation.add_error("Please enter a valid Date of Birth", false);
                }
                else if (View.DateOfBirth.toDateTime().occursOnOrAfter(DateTime.Today))
                {
                    validation.add_error("Date of Birth must be less than today's date", false);
                }
                else
                {
                    person.BirthDate = View.DateOfBirth.toDateTime();
                }
            }
            else
            {
                person.BirthDate = null; // do we really want to do this?
            }
        }

        if (View.NickNameVisible)
        {
            person.Nickname = View.Nickname;
        }

        // character validation
        validation.combine_with(person.PerformCharacterValidation(first_name_required: false, last_name_required: false));

        // title case
        person.TitleCaseFields();
    }

    private void GetParticipant(ref PCentralParticipant entity, ref PCentralValidationResults boRequiredFields)
    {
        entity.Status = Enumerations.enumParticipantStatuses.Active;
        entity.ProgramID = ContextManager.ProgramID;
        entity.PrimaryFlg = false;
        entity.ProgramTermsAndConditionsFlg = true;
        entity.StatusDT = DateTime.Now;
        entity.PartyID = ContextManager.PartyID;
        entity.ParticipantID = ContextManager.CurrentGuestParticipantID;

        //ms HDS change request # changing CertifiedCopyBirthCertificate to PhotoID
        if (!View.PassportVisible)
        {
            entity.ProofOfCitizenshipType = Enumerations.enumProofofCitizenshipTypes.PhotoID;
        }
        else if (View.PassportVisible && View.POCTypePassportOnly)
        {
            entity.ProofOfCitizenshipType = Enumerations.enumProofofCitizenshipTypes.Passport;
        }
        else if (View.PassportVisible)
        {
            entity.ProofOfCitizenshipType = View.GetSetProofofCitizenshipType;
        }

        if (View.PassportVisible)
        {
            if (View.HasPassportInformation == enumTriBoolean.tbTrue)
            {
                //If View.PassportStatusAvailable Then
                entity.PassportStatus = Enumerations.enumPassportStatuses.Available;
            }
            else if (View.HasPassportInformation == enumTriBoolean.tbFalse)
            {
                //ElseIf Not View.PassportStatusAvailable Then
                entity.PassportStatus = Enumerations.enumPassportStatuses.NotAvailable;
            }

            if (entity.ProofOfCitizenshipType == Enumerations.enumProofofCitizenshipTypes.Passport && entity.PassportStatus.toInt() == 0)
            {
                Utilities.CheckRequired(entity.PassportStatus.ToString(), "Passport Status", ref boRequiredFields);
            }
        }
    }

    private void loadData(PCentralParticipant participant)
    {
        //if not active, leave this place - go forth and prosper.
        if (participant != null && participant.InactiveType.toInt() > 0)
        {
            Utilities.Navigate(Utilities.NextPage("inactive_guest"));
        }

        if (View.EditableLegalNameVisible)
        {
            View.PrefixList = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.NamePrefixes).Tables[0];
        }
        if (View.GenderVisible)
        {
            View.GenderList = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.ISOGenderCodes).Tables[0];
        }
        if (View.ProofOfCitizenshipVisible)
        {
            View.CountryList = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.ISOCountryCodes).Tables[0];
        }
        if (View.ProofOfCitizenshipVisible)
        {
            View.ProofofCitizenshipTypeList = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.ProofofCitizenshipTypes).Tables[0];
        }

        View.SuffixList = PCentralCodeLookupDAL.ReadCodeLookup(Utilities.GetEnvironmentConnection(), Enumerations.enumCodeTypes.NameSuffixes).Tables[0];
        LoadPerson(participant.Person);
        LoadProfileInfo(participant.Person.PersonProfile);
        LoadParticipant(participant);
        LoadProofOfCitizenShip(participant.Person.ProofOfCitizenship);
    }

    private void LoadPerson(PCentralPerson person)
    {
        if (View.EditableLegalNameVisible)
        {
            View.LegalFirstName = person.LegalFirstName;
            View.LegalMiddleName = person.LegalMiddleName;
            View.LegalLastName = person.LegalLastName;
            View.LegalPrefix = person.LegalPrefix.toInt(0).ToString();
            View.LegalSuffix = person.LegalSuffix.toInt(0).ToString();
        }

        View.LegalName = person.FullName;
        View.Nickname = person.Nickname;

        if (person.BirthDate.isNotEmptyDate())
        {
            View.DateOfBirth = ((DateTime)person.BirthDate).ToString(ContextManager.DateFormat);
        }

        View.GetSetGender = ((int)person.GenderID).ToString();
    }

    private void LoadProfileInfo(PCentralPersonProfile profile)
    {
        if (profile == null) return;

        View.BadgeFirstName = profile.BadgeFirstName;
        View.BadgeLastName = profile.BadgeLastName;

        View.DietaryPreferences = profile.DietaryPreferences;
        View.PhysicalAssistanceRequests = profile.PhysicalAssistance;
    }

    private void LoadParticipant(PCentralParticipant participant)
    {
        if (participant.ProofOfCitizenshipType.toInt() > 0 && View.ProofOfCitizenshipVisible)
        {
            View.GetSetProofofCitizenshipType = participant.ProofOfCitizenshipType ?? default(Enumerations.enumProofofCitizenshipTypes);
        }

        if (participant.PassportStatus == Enumerations.enumPassportStatuses.Available && View.PassportVisible)
        {
            View.HasPassportInformation = enumTriBoolean.tbTrue;
        }
        else if (participant.PassportStatus == Enumerations.enumPassportStatuses.NotAvailable)
        {
            View.HasPassportInformation = enumTriBoolean.tbFalse;
        }
    }

    private void LoadProofOfCitizenShip(PCentralProofOfCitizanship poc)
    {
        if (poc != null)
        {
            if (poc.BirthCertificateNationalityID.toInt() > 0 && View.BirthCertificateVisible)
            {
                View.GetSetBirthCertificate = poc.BirthCertificateNationalityID.ToString();
            }

            if (View.PassportVisible)
            {
                View.FirstPassportName = poc.PassportFirstName;
                View.MiddlePassportName = poc.PassportMiddleName;
                View.LastPassportName = poc.PassportLastName;
                View.PassportNumber = poc.PassportNumber;
                View.PassportIssuingAuthority = poc.IssuingAuthority;
                // View.PassportCityOfIssue = .CityOfIssue

                if (poc.PassportSuffixID > 0)
                {
                    View.GetSetSuffixPassport = poc.PassportSuffixID.ToString();
                }

                if (poc.PassportExpirationDate.isNotEmptyDate())
                {
                    View.PassportExpirationDate = ((DateTime)poc.PassportExpirationDate).ToString(ContextManager.DateFormat);
                }

                if (poc.PassportIssueDate.isNotEmptyDate())
                {
                    View.PassportIssueDate = ((DateTime)poc.PassportIssueDate).ToString(ContextManager.DateFormat);
                }

                if (poc.PassportNumber != null && poc.PassportNumber.Length > 0)
                {
                    if (poc.PassportNationalityID.toInt() > 0)
                    {
                        View.GetSetNationality = poc.PassportNationalityID.toInt(0).ToString();
                    }

                    if (poc.PassportCountryOfIssueID.toInt() > 0)
                    {
                        View.GetSetCountryOfIssue = poc.PassportCountryOfIssueID.toInt(0).ToString();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Add custom fields to View.ProfileCustomFields
    /// </summary>
    /// <param name="CustomFieldNames">; delimited</param>
    /// <param name="Required"></param>
    private void AddProfileCustomFields(string CustomFieldNames, bool Required = false)
    {
        List<string> Fields = View.ProfileCustomFields.Split(';').ToList();
        List<string> NewFields = CustomFieldNames.Split(';').ToList();

        foreach (string CustomFieldName in NewFields)
        {
            if (!(string.IsNullOrWhiteSpace(CustomFieldName) || Fields.Contains(CustomFieldName)))
            {
                Fields.Add(CustomFieldName);
                View.ProfileCustomFields = string.Join(";", Fields);
                View.ProfileCustomFieldsRequired = View.ProfileCustomFieldsRequired + (string.IsNullOrEmpty(View.ProfileCustomFieldsRequired) ? "" : ";") + Required.ToString();
            }
        }
    }

    #endregion

}

#endregion

#region " ### DTO ### "

public class GuestProfileInformationDTO
{

    private bool _validatePassportName;
    private string _prefixtext;
    private bool _prefixvisible = true;
    private bool _welcometextvisible = true;
    private string _welcometext;
    private bool _informationtextvisible = true;
    private string _informationtext;
    private bool _generalInfoSectionVisible = true;
    private string _generalInfoSectionHeadingText;
    private bool _nameSubSectionVisible = true;
    private string _nameSubSectionText;
    private bool _editableLegalNameVisible = true;
    private bool _displayNameVisible = true;
    private string _displayNameText;
    private bool _suffixvisible = true;
    private string _suffixtext;
    private bool _profileheadervisible = true;
    private string _profileheadertext;
    private bool _nickNameVisible = true;
    private string _nickNametext;
    private bool _badgeNameVisible = true;
    private string _badgeFNameText;
    private string _badgeLNameText;
    private bool _requireBadgeFName = true;
    private bool _requireBadgeLName = true;
    //private string _awardInfoText;
    //private bool _awardInfoVisible = true;
    //private string _personalInfoSubSectionText;
    private bool _genderVisible = true;
    private bool _requireGender = true;
    private string _lblGendertext;
    private bool _DOBVisible = true;
    private bool _requireDOB = true;
    private string _lblDOBtext;
    //POC
    private bool _POCvisible = true;
    private string _POCSectionHeader;
    private string _POCSubSection;
    private bool _POCRequired = true;
    private string _POCTypeText;
    private bool _POCTypePassportOnly;
    private string _POCCountryText;
    //Passport
    private string _passportSubSection;
    private string _passportAvailableText;
    private string _passportStatusYesText;
    private string _passportStatusNoText;
    private string _passportFNameText;
    private string _passportMNameText;
    private string _passportLNameText;
    private string _passportSuffixText;
    private string _passportNbrText;
    private string _passportExpDateText;
    private string _passportNationText;
    private string _passportIssuedText;
    private bool _passportVisible = true;
    //Birth Cert
    private string _birthCSubSection;
    private string _bCertNationText;
    private bool _birthCertificateVisible = true;
    //Special Request
    private bool _specialRequestVisible = true;
    private string _specialReqSubSection;
    private bool _specialReqSubSectionVisible = true;
    private string _dietPrefText;
    private string _physicalReqText;
    //private int _allowedNumberOfGuests;
    private string _lblLegalNameVerbiageText;
    private bool _lblLegalNameVerbiageVisible = true;


    /// <summary>
    /// Constructor used by read db settings method to populate DTO.
    /// </summary>
    /// <param name="settingsds"></param>
    /// <remarks></remarks>
    public GuestProfileInformationDTO(DataSet settingsds)
    {
        if ((settingsds != null) && (settingsds.Tables[1] != null))
        {
            foreach (DataRow row in settingsds.Tables[1].Rows)
            {
                //because this is not strongly typed, there is no point of using hashtable
                //set up your properties so its easier to call it from the presenter
                switch (row["ItemName"].ToString())
                {
                    case "validatepassportname":
                        this._validatePassportName = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "prefixtext":
                        this._prefixtext = row["ItemValue"].ToString();
                        break;
                    case "prefixvisible":
                        this._prefixvisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "welcometextvisible":
                        this._welcometextvisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "welcometext":
                        this._welcometext = row["ItemValue"].ToString();
                        break;
                    case "informationtextvisible":
                        this._informationtextvisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "informationtext":
                        this._informationtext = row["ItemValue"].ToString();
                        break;
                    case "generalinfosectionvisible":
                        this._generalInfoSectionVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "generalinfosectionheadingtext":
                        this._generalInfoSectionHeadingText = row["ItemValue"].ToString();
                        break;
                    case "namesubsectionvisible":
                        this._nameSubSectionVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "namesubsectiontext":
                        this._nameSubSectionText = row["ItemValue"].ToString();
                        break;
                    case "editablelegalnamevisible":
                        this._editableLegalNameVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "displaynamevisible":
                        this._displayNameVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "displaynametext":
                        this._displayNameText = row["ItemValue"].ToString();
                        break;
                    case "suffixvisible":
                        this._suffixvisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "suffixtext":
                        this._suffixtext = row["ItemValue"].ToString();
                        break;
                    case "nicknamevisible":
                        this._nickNameVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "nicknametext":
                        this._nickNametext = row["ItemValue"].ToString();
                        break;
                    case "badgenamevisible":
                        this._badgeNameVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "badgefnametext":
                        this._badgeFNameText = row["ItemValue"].ToString();
                        break;
                    case "badgelnametext":
                        this._badgeLNameText = row["ItemValue"].ToString();
                        break;
                    case "requirebadgefname":
                        this._requireBadgeFName = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "requirebadgelname":
                        this._requireBadgeLName = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "profileheadervisible":
                        this._profileheadervisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "profileheadertext":
                        this._profileheadertext = row["ItemValue"].ToString();
                        break;
                    case "gendervisible":
                        this._genderVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "requiregender":
                        this._requireGender = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "lblgendertext":
                        this._lblGendertext = row["ItemValue"].ToString();
                        break;
                    case "dobvisible":
                        this._DOBVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "requiredob":
                        this._requireDOB = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "lbldobtext":
                        this._lblDOBtext = row["ItemValue"].ToString();
                        break;
                    //POC
                    case "pocvisible":
                        this._POCvisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "pocsectionheader":
                        this._POCSectionHeader = row["ItemValue"].ToString();
                        break;
                    case "poccountryofbirthvisible":
                        this._POCCountryofBirthVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "pocsubsection":
                        this._POCSubSection = row["ItemValue"].ToString();
                        break;
                    case "pocrequired":
                        this._POCRequired = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "poctypetext":
                        this._POCTypeText = row["ItemValue"].ToString();
                        break;
                    case "poctypepassportonly":
                        this._POCTypePassportOnly = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "poccountrytext":
                        this._POCCountryText = row["ItemValue"].ToString();
                        break;
                    //Passport
                    case "passportsubsection":
                        this._passportSubSection = row["ItemValue"].ToString();
                        break;
                    case "passportavailabletext":
                        this._passportAvailableText = row["ItemValue"].ToString();
                        break;
                    case "passportstatusyestext":
                        this._passportStatusYesText = row["ItemValue"].ToString();
                        break;
                    case "passportstatusnotext":
                        this._passportStatusNoText = row["ItemValue"].ToString();
                        break;
                    case "passportfnametext":
                        this._passportFNameText = row["ItemValue"].ToString();
                        break;
                    case "passportmnametext":
                        this._passportMNameText = row["ItemValue"].ToString();
                        break;
                    case "passportlnametext":
                        this._passportLNameText = row["ItemValue"].ToString();
                        break;
                    case "passportsuffixtext":
                        this._passportSuffixText = row["ItemValue"].ToString();
                        break;
                    case "passportnbrtext":
                        this._passportNbrText = row["ItemValue"].ToString();
                        break;
                    case "passportexpdatetext":
                        this._passportExpDateText = row["ItemValue"].ToString();
                        break;
                    case "passportissuedatetext":
                        this._passportissuedatetext = row["ItemValue"].ToString();
                        break;
                    case "passportcityissuetext":
                        this._passportcityissuetext = row["ItemValue"].ToString();
                        break;
                    case "passportissueauthoritytext":
                        this._passportissueauthoritytext = row["ItemValue"].ToString();
                        break;
                    case "passportnationtext":
                        this._passportNationText = row["ItemValue"].ToString();
                        break;
                    case "passportsissuedtext":
                        this._passportIssuedText = row["ItemValue"].ToString();
                        break;
                    case "passportvisible":
                        this._passportVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    //Birth Cert
                    case "birthcsubsection":
                        this._birthCSubSection = row["ItemValue"].ToString();
                        break;
                    case "bcertnationtext":
                        this._bCertNationText = row["ItemValue"].ToString();
                        break;
                    case "birthcertificatevisible":
                        this._birthCertificateVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    //Special Request
                    case "specialrequestvisible":
                        this._specialRequestVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "specialreqsubsection":
                        this._specialReqSubSection = row["ItemValue"].ToString();
                        break;
                    case "specialreqsubsectionvisible":
                        this._specialReqSubSectionVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "dietpreftext":
                        this._dietPrefText = row["ItemValue"].ToString();
                        break;
                    case "physicalreqtext":
                        this._physicalReqText = row["ItemValue"].ToString();
                        break;
                    //cf
                    case "profilecustomfields":
                        this._profilecustomfields = row["ItemValue"].ToString();
                        break;
                    case "profilecustomfieldsrequired":
                        this._profilecustomfieldsrequired = row["ItemValue"].ToString();
                        break;
                    case "verbiageLegalNameVisible":
                        this._lblLegalNameVerbiageVisible = Convert.ToBoolean(row["ItemValue"]);
                        break;
                    case "verbiageLegalNameText":
                        this._lblLegalNameVerbiageText = row["ItemValue"].ToString();
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Constructor used for user defined settings.
    /// </summary>
    /// <remarks></remarks>
    public GuestProfileInformationDTO()
    {

    }
    private bool _POCCountryofBirthVisible = true;
    public bool POCCountryofBirthVisible
    {
        get { return _POCCountryofBirthVisible; }
        set { _POCCountryofBirthVisible = value; }
    }
    private string _passportissueauthoritytext;
    public string Passportissueauthoritytext
    {
        get { return _passportissueauthoritytext; }
        set { _passportissueauthoritytext = value; }
    }

    private string _passportcityissuetext;
    public string Passportcityissuetext
    {
        get { return _passportcityissuetext; }
        set { _passportcityissuetext = value; }
    }

    private string _passportissuedatetext;
    public string Passportissuedatetext
    {
        get { return _passportissuedatetext; }
        set { _passportissuedatetext = value; }
    }
    private string _profilecustomfieldsrequired;
    /// <summary>
    /// Collection of ; delimited True/False values for each item in Preprofilecustomfields property.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string Profilecustomfieldsrequired
    {
        get { return _profilecustomfieldsrequired; }
        set { _profilecustomfieldsrequired = value; }
    }

    private string _profilecustomfields;
    /// <summary>
    /// Collection of ; delimited Custom Fields names.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public string Profilecustomfields
    {
        get { return _profilecustomfields; }
        set { _profilecustomfields = value; }
    }
    //Special Request
    public string PhysicalReqText
    {
        get { return _physicalReqText; }
        set { _physicalReqText = value; }
    }
    public string DietPrefText
    {
        get { return _dietPrefText; }
        set { _dietPrefText = value; }
    }
    public string SpecialReqSubSection
    {
        get { return _specialReqSubSection; }
        set { _specialReqSubSection = value; }
    }
    public bool SpecialReqSubSectionVisible
    {
        get { return _specialReqSubSectionVisible; }
        set { _specialReqSubSectionVisible = value; }
    }
    public bool SpecialRequestVisible
    {
        get { return _specialRequestVisible; }
        set { _specialRequestVisible = value; }
    }
    //Birth Cert
    public string BCertNationText
    {
        get { return _bCertNationText; }
        set { _bCertNationText = value; }
    }
    public string BirthCSubSection
    {
        get { return _birthCSubSection; }
        set { _birthCSubSection = value; }
    }
    public bool BirthCertificateVisible
    {
        get { return _birthCertificateVisible; }
        set { _birthCertificateVisible = value; }
    }
    //Passport
    public string PassportIssuedText
    {
        get { return _passportIssuedText; }
        set { _passportIssuedText = value; }
    }
    public string PassportNationText
    {
        get { return _passportNationText; }
        set { _passportNationText = value; }
    }
    public string PassportExpDateText
    {
        get { return _passportExpDateText; }
        set { _passportExpDateText = value; }
    }
    public string PassportNbrText
    {
        get { return _passportNbrText; }
        set { _passportNbrText = value; }
    }
    public string PassportSuffixText
    {
        get { return _passportSuffixText; }
        set { _passportSuffixText = value; }
    }
    public string PassportLNameText
    {
        get { return _passportLNameText; }
        set { _passportLNameText = value; }
    }
    public string PassportMNameText
    {
        get { return _passportMNameText; }
        set { _passportMNameText = value; }
    }
    public string PassportFNameText
    {
        get { return _passportFNameText; }
        set { _passportFNameText = value; }
    }
    public string PassportStatusNoText
    {
        get { return _passportStatusNoText; }
        set { _passportStatusNoText = value; }
    }
    public string PassportStatusYesText
    {
        get { return _passportStatusYesText; }
        set { _passportStatusYesText = value; }
    }
    public string PassportAvailableText
    {
        get { return _passportAvailableText; }
        set { _passportAvailableText = value; }
    }
    public string PassportSubSection
    {
        get { return _passportSubSection; }
        set { _passportSubSection = value; }
    }
    //Passport
    public bool PassportVisible
    {
        get { return _passportVisible; }
        set { _passportVisible = value; }
    }
    //POC
    public string POCCountryText
    {
        get { return _POCCountryText; }
        set { _POCCountryText = value; }
    }
    public bool POCTypePassportOnly
    {
        get { return _POCTypePassportOnly; }
        set { _POCTypePassportOnly = value; }
    }
    public string POCTypeText
    {
        get { return _POCTypeText; }
        set { _POCTypeText = value; }
    }
    public bool POCRequired
    {
        get { return _POCRequired; }
        set { _POCRequired = value; }
    }
    public string POCSubSection
    {
        get { return _POCSubSection; }
        set { _POCSubSection = value; }
    }
    public string POCSectionHeader
    {
        get { return _POCSectionHeader; }
        set { _POCSectionHeader = value; }
    }
    public bool POCVisible
    {
        get { return _POCvisible; }
        set { _POCvisible = value; }
    }
    public bool GenderVisible
    {
        get { return _genderVisible; }
        set { _genderVisible = value; }
    }
    public bool GenderRequire
    {
        get { return _requireGender; }
        set { _requireGender = value; }
    }
    public bool DOBVisible
    {
        get { return _DOBVisible; }
        set { _DOBVisible = value; }
    }
    public bool DOBRequire
    {
        get { return _requireDOB; }
        set { _requireDOB = value; }
    }
    public bool NicknameVisible
    {
        get { return _nickNameVisible; }
        set { _nickNameVisible = value; }
    }
    public string NickNameText
    {
        get { return _nickNametext; }
        set { _nickNametext = value; }
    }
    public bool BadgeNameVisible
    {
        get { return _badgeNameVisible; }
        set { _badgeNameVisible = value; }
    }
    public string DisplayNameText
    {
        get { return _displayNameText; }
        set { _displayNameText = value; }
    }
    public bool NameSubSectionVisible
    {
        get { return _nameSubSectionVisible; }
        set { _nameSubSectionVisible = value; }
    }
    public string NameSubSectionText
    {
        get { return _nameSubSectionText; }
        set { _nameSubSectionText = value; }
    }
    public bool GeneralInfoSectionVisible
    {
        get { return _generalInfoSectionVisible; }
        set { _generalInfoSectionVisible = value; }
    }
    public string GeneralInfoSectionHeadingText
    {
        get { return _generalInfoSectionHeadingText; }
        set { _generalInfoSectionHeadingText = value; }
    }
    public string BadgeFNameText
    {
        get { return _badgeFNameText; }
        set { _badgeFNameText = value; }
    }
    public string BadgeLNameText
    {
        get { return _badgeLNameText; }
        set { _badgeLNameText = value; }
    }
    public bool Prefixvisible
    {
        get { return _prefixvisible; }
        set { _prefixvisible = value; }
    }
    public string Prefixtext
    {
        get { return _prefixtext; }
        set { _prefixtext = value; }
    }
    public bool WelcometextVisible
    {
        get { return _welcometextvisible; }
        set { _welcometextvisible = value; }
    }
    public string WelcomeText
    {
        get { return _welcometext; }
        set { _welcometext = value; }
    }
    public bool InformationTextVisible
    {
        get { return _informationtextvisible; }
        set { _informationtextvisible = value; }
    }
    public string Informationtext
    {
        get { return _informationtext; }
        set { _informationtext = value; }
    }
    public bool Suffixvisible
    {
        get { return _suffixvisible; }
        set { _suffixvisible = value; }
    }
    public string Suffixtext
    {
        get { return _suffixtext; }
        set { _suffixtext = value; }
    }
    public bool ProfileHeadervisible
    {
        get { return _profileheadervisible; }
        set { _profileheadervisible = value; }
    }
    public string ProfileHeadertext
    {
        get { return _profileheadertext; }
        set { _profileheadertext = value; }
    }
    public string Gendertext
    {
        get { return _lblGendertext; }
        set { _lblGendertext = value; }
    }
    public string DOBtext
    {
        get { return _lblDOBtext; }
        set { _lblDOBtext = value; }
    }
    public bool ValidatePassportName
    {
        get { return Utilities.ReadCustomConfigValue("PassportNameValidation").toBool(false); }
    }
    public bool EditableLegalNameVisible
    {
        get { return _editableLegalNameVisible; }
        set { _editableLegalNameVisible = value; }
    }
    public bool RequireBadgeFName
    {
        get { return _requireBadgeFName; }
        set { _requireBadgeFName = value; }
    }
    public bool RequireBadgeLName
    {
        get { return _requireBadgeLName; }
        set { _requireBadgeLName = value; }
    }
    public bool DisplayNameVisible
    {
        get { return _displayNameVisible; }
        set { _displayNameVisible = value; }
    }
    public string LblLegalNameVerbiageText
    {
        get { return _lblLegalNameVerbiageText; }
        set { _lblLegalNameVerbiageText = value; }
    }
    public bool LblLegalNameVerbiageVisible
    {
        get { return _lblLegalNameVerbiageVisible; }
        set { _lblLegalNameVerbiageVisible = value; }
    }

}

#endregion
