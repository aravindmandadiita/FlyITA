using System.Linq;
using System.Text;
using PCentralLib;
using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using ITALib;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Web.UI.HtmlControls;
using PCentralLib.custom_fields;
using PCentralLib.ExactTargetClient;
using PCentralLib.person;

namespace FlyITA
{
    partial class Travelerprofileinformation : PageBase
    {


        #region " ### Page Events ### "

        readonly List<string> _airLines = new List<string>();
        readonly List<string> hotels = new List<string>();
        readonly List<string> rentalcars = new List<string>();
        protected void Page_PreRender(System.Object sender, System.EventArgs e)
        {

        }

        protected void Page_Load(System.Object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                List<string> ActiveGuestlist = new List<string>();
                ActiveGuestlist.Add("1");

                rptPerson.DataSource = ActiveGuestlist;
                rptPerson.DataBind();

                rptPersonHotelClubMemberships.DataSource = ActiveGuestlist;
                rptPersonHotelClubMemberships.DataBind();

                rptRentalCar.DataSource = ActiveGuestlist;
                rptRentalCar.DataBind();
               
            }


           
 
        }


        protected void rptPerson_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindGuestData(e.Item);
        }

        protected void rptPersonHotelClubMemberships_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindHotelClubMembershipsData(e.Item);
        }
        protected void rptRentalCar_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindrentalCars(e.Item);
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                cptCaptcha.ValidateCaptcha(txtCaptcha.Text.Trim());
                if (!cptCaptcha.UserValidated)
                {
                    UserMessages.Text = "Invalid Captcha Text.";
                    return;
                }

                PCentralValidationResults validation = new PCentralValidationResults();
                if (ValidateInput(validation)) return;

                SendTravelerProfileEmail();
                Utilities.Navigate("Thankyou.aspx");
            }
            catch (Exception ex)
            {
                UserMessages.Text = ex.Message;
                throw;
            }


        }

        private bool ValidateInput(PCentralValidationResults validation)
        {
            if (!BirthDate.isDateTime())
            {
                validation.add_warning("Please enter a valid Date of Birth");
            }
            else if (BirthDate.toDateTime() > DateTime.Today)
            {
                validation.add_warning("Date of Birth must be less than today's date");
            }

            if (!Utilities.IsNothingNullOrEmpty(PassportExpirationDate) && !PassportExpirationDate.isDateTime())
            {
                validation.add_warning("Please enter a valid Passport Expiration Date");
            }

            if (!Utilities.IsNothingNullOrEmpty(PassportIssueDate) && PassportIssueDate.isDateTime())
            {
              
                if (PassportExpirationDate.isDateTime() && PassportIssueDate.toDateTime() > PassportExpirationDate.toDateTime())
                {
                    validation.add_warning("Your Passport Issue Date can not be greater than passport Expiration Date.");
                }
                if (PassportIssueDate.toDateTime(DateTime.MinValue) > DateTime.Today)
                {
                    validation.add_warning("Your Passport Issue Date can not be greater than today's date.");
                }
            }
            else
            {
                if (!Utilities.IsNothingNullOrEmpty(PassportIssueDate))
                validation.add_warning("Please enter a valid Passport Issue Date");
            }
            foreach (FrequentFlyerNumberClass flyerNumber in FrequentFlyerNumber)
            {
                if (!Utilities.IsNothingNullOrEmpty(flyerNumber.FrequentFlyerNumber) && !flyerNumber.FrequentFlyerNumber.Contains("Select"))
                {
                    if (Utilities.IsNothingNullOrEmpty(flyerNumber.FrequentFlyerNumberId))
                    {
                        validation.add_warning("If an Airline is selected, a corresponding Frequent Flyer Number is required.");
                    }
                }
            }
            foreach (HotelMembershipClass HoteMemberships in HotelClubMemberships)
            {
                if (!Utilities.IsNothingNullOrEmpty(HoteMemberships.HotelName) && !HoteMemberships.HotelName.Contains("Select"))
                {
                    if (Utilities.IsNothingNullOrEmpty(HoteMemberships.HotelId))
                    {
                        validation.add_warning("If an Hotel is selected, a corresponding Hotel Memberships Number is required.");
                    }
                }
            }
            foreach (RentalCarClass reltaCarClass in RentalCar)
            {
                if (!Utilities.IsNothingNullOrEmpty(reltaCarClass.RentalCarName) && !reltaCarClass.RentalCarName.Contains("Select"))
                {
                    if (Utilities.IsNothingNullOrEmpty(reltaCarClass.RentalCarId))
                    {
                        validation.add_warning("If an Rental Car is selected, a corresponding Rental Car Number is required.");
                    }
                }
            }

            if (!Utilities.IsNothingNullOrEmpty(txtOtherHotelMemberShip.Text))
            {
                if (Utilities.IsNothingNullOrEmpty(txtOtherHotelClubMembershipsNumber.Text))
                {
                    validation.add_warning("If an Other Hotel is entered, a corresponding Other Hotel Memberships Number is required.");
                }
            }
            
 

            if (validation.has_errors_or_warnings)
            {
                UserMessages.CssClass = "warning";
                UserMessages.Text = Utilities.DisplayAllErrorsAndWarningsbr(validation);
                return true;
            }
             
            return false;
        }

        #endregion

        #region " ### Model Properties ### "


        public string TravelerFirstName
        {
            get { return this.txtTravelerFirstName.Text; }
        }

        public string TravelerLastName
        {
            get { return this.txtTravelerLastName.Text; }
        }
        public string TravelerMiddleName
        {
            get { return txtTravelerMiddleName.Text; }

        }
        public string CompanyName
        {
            get { return txtCompanyName.Text; }

        }

        public string TravelerTitle
        {
            get { return txtTitle.Text; }

        }

        public string DeptCostCenter
        {
            get { return txtDeptCostCenter.Text; }

        }
        public string EmailAddress
        {
            get { return txtEmailAddress.Text; }

        }
        public string BusinessPhone
        {
            get { return txtBusinessPhone.Text; }

        }
        public string BusinessFax
        {
            get { return txtBusinessFax.Text; }
        }
        public string MobilePhone
        {
            get { return txtMobilePhone.Text; }

        }
        public string HomePhone
        {
            get { return txtHomePhone.Text; }

        }
        public string BirthDate
        {
            get { return txtDateOfBirth.Text; }

        }
        public string Gender
        {
            get { return ddlGender.SelectedItem.Text; }

        }

        public string PassportName
        {
            get { return txtPassportName.Text; }

        }
        public string PassportNumber
        {
            get { return txtPassportNumber.Text; }

        }

        public string PassportIssueDate
        {
            get
            {

                return txtPassportIssueDate.Text;
            }

        }

        public string PassportExpirationDate
        {
            get { return txtExpirationDate.Text; }

        }
        public string PlaceofIssue
        {
            get { return txtPlaceofIssue.Text; }

        }
        public string TravelerArrangerName
        {
            get { return txtTravelerArrangerName.Text; }

        }
        public string TravelerArrangerPhone
        {
            get { return txtTravelerArrangerPhone.Text; }

        }
        public string TravelerArrangerEmail
        {
            get { return txtTravelerArrangerEmail.Text; }

        }


        public string EmergencyContactName
        {
            get { return txtContactName.Text; }

        }

        public string EmergencyContactRelationship
        {
            get { return ddlContactRelationship.SelectedItem.Text; }

        }

        public string EmergencyContactPhone
        {
            get { return txtEmergencyContactPhone.Text; }

        }
        public string PreferredDepartureAirport
        {
            get { return ddlPreferredDepartureAirport.SelectedItem.Text; }

        }

        public string PreferredCarrier
        {
            get
            {
                return ddlPreferredCarrier.SelectedItem.Text.Contains("Select") ? string.Empty : ddlPreferredCarrier.SelectedItem.Text;
            }
        }
        public string OtherPreferredCarrier
        {
            get { return txtOtherPreferredCarrier.Text; }

        }
        public string SeatingPreference
        {
            get { return ddlSeatingPreference.SelectedItem.Text; }

        }
        public string SpecialMealRequirements
        {
            get { return txtSpecialMealRequirements.Text; }

        }

        public class FrequentFlyerNumberClass
        {
            public string FrequentFlyerNumber { get; set; }
            public string FrequentFlyerNumberId { get; set; }
        }
        public class HotelMembershipClass
        {
            public string HotelName { get; set; }
            public string HotelId { get; set; }
           
        }
        public class RentalCarClass
        {
            public string RentalCarName { get; set; }
            public string RentalCarId { get; set; }
        }

        public List<FrequentFlyerNumberClass> FrequentFlyerNumber
        {
            get
            {
                List<FrequentFlyerNumberClass> ht = new List<FrequentFlyerNumberClass>();

           
                foreach (RepeaterItem riPerson in this.rptPerson.Items)
                {
                    foreach (RepeaterItem riFrequentFlyerNumber in ((Repeater)riPerson.FindControl("rptFrequentFlyerNumber")).Items)
                    {
                        if (!((DropDownList)riFrequentFlyerNumber.FindControl("ddlFrequentFlyerNumber")).SelectedItem.Text.Contains("Select"))
                        {
                            string selectedFrequentFlyerNumber = ((DropDownList)riFrequentFlyerNumber.FindControl("ddlFrequentFlyerNumber")).SelectedItem.Text;
                            string selectedFrequentFlyerNumberId = ((TextBox)riFrequentFlyerNumber.FindControl("txtFrequentFlyerNumber")).Text;
                            //ht.Add(selectedFrequentFlyerNumber, selectedFrequentFlyerNumberId);
                            ht.Add(new FrequentFlyerNumberClass {FrequentFlyerNumber= selectedFrequentFlyerNumber,FrequentFlyerNumberId= selectedFrequentFlyerNumberId });

                        }
                    }
                }

                return ht;
            }

        }
        public string SmokingPreference
        {
            get { return ddlSmokingPreference.SelectedItem.Text; }

        }
        public string BedPreference
        {
            get { return ddlBedPreference.SelectedItem.Text; }

        }
        public string SpecialRequirements
        {
            get { return txtSpecialRequirements.Text; }

        }     
        public List<HotelMembershipClass> HotelClubMemberships
        {
            get
            {
                List<HotelMembershipClass> ht = new List<HotelMembershipClass>();
               
                foreach (RepeaterItem riPersonHotelClubMemberships in this.rptPersonHotelClubMemberships.Items)
                {
                    foreach (RepeaterItem riHotelClubMembershipsNumber in ((Repeater)riPersonHotelClubMemberships.FindControl("rptHotelClubMembershipsNumber")).Items)
                    {
                        if (!((DropDownList)riHotelClubMembershipsNumber.FindControl("ddlHotelClubMemberships")).SelectedItem.Text.Contains("Select"))
                        {
                            string selectedHotelClubMembership = ((DropDownList)riHotelClubMembershipsNumber.FindControl("ddlHotelClubMemberships")).SelectedItem.Text;
                            string hotelClubMembershipsId = ((TextBox)riHotelClubMembershipsNumber.FindControl("txtHotelClubMemberships")).Text;
                         
                            ht.Add(new HotelMembershipClass { HotelName = selectedHotelClubMembership, HotelId = hotelClubMembershipsId });
                        }
                    }
                }

                return ht;
            }

        }

        public string VehicleSize
        {
            get { return ddlVehicleSize.SelectedValue; }

        }
         
        public List<RentalCarClass> RentalCar
        {
            get
            {
                List<RentalCarClass> ht = new List<RentalCarClass>();


                foreach (RepeaterItem riRentalCar in this.rptRentalCar.Items)
                {
                    foreach (RepeaterItem RentalCarMemberships in ((Repeater)riRentalCar.FindControl("rptRentalCarMemberships")).Items)
                    {
                        if (!((DropDownList)RentalCarMemberships.FindControl("ddlRentalCarMemberships")).SelectedItem.Text.Contains("Select"))
                        {
                            string selectedRentalCarMemberships = ((DropDownList)RentalCarMemberships.FindControl("ddlRentalCarMemberships")).SelectedItem.Text;
                            string RentalCarMembershipsId = ((TextBox)RentalCarMemberships.FindControl("txtRentalCarMemberships")).Text;

                            ht.Add(new RentalCarClass { RentalCarName = selectedRentalCarMemberships, RentalCarId = RentalCarMembershipsId });

                        }
                    }
                }

                return ht;
            }

        }


        public object CurrentPage
        {
            get { return Utilities.GetCurrentPage().Split('.')[0]; }
        }
        protected void rptFrequentFlyerNumber_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindFrequentFlyerTypes(e.Item);
        }
        protected void rptHotelClubMembershipsNumber_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindHotelClubMembershipsNumber(e.Item);
        }
        protected void rptRentalCarMemberships_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataRentalCarMemberships(e.Item);
        }

        private void dataBindGuestData(System.Web.UI.WebControls.RepeaterItem ri)
        {
            Repeater rpt;
            List<string> ActiveGuestlist = new List<string>();
            ActiveGuestlist.Add("1");
            ActiveGuestlist.Add("2");
            ActiveGuestlist.Add("3");
            ActiveGuestlist.Add("4");
            ActiveGuestlist.Add("5");
            rpt = (Repeater)ri.FindControl("rptFrequentFlyerNumber");
            rpt.DataSource = ActiveGuestlist;
            rpt.DataBind();
        }

        private void dataBindHotelClubMembershipsData(System.Web.UI.WebControls.RepeaterItem ri)
        {
            Repeater rpt;
            List<string> ActiveGuestlist = new List<string>();
            ActiveGuestlist.Add("1");
            ActiveGuestlist.Add("2");
            ActiveGuestlist.Add("3");
            ActiveGuestlist.Add("4");
            ActiveGuestlist.Add("5");
            rpt = (Repeater)ri.FindControl("rptHotelClubMembershipsNumber");
            rpt.DataSource = ActiveGuestlist;
            rpt.DataBind();
        }
        private void dataBindrentalCars(System.Web.UI.WebControls.RepeaterItem ri)
        {
            Repeater rpt;
            List<string> ActiveGuestlist = new List<string>();
            ActiveGuestlist.Add("1");
            ActiveGuestlist.Add("2");
            ActiveGuestlist.Add("3");
            ActiveGuestlist.Add("4");
            ActiveGuestlist.Add("5");
            rpt = (Repeater)ri.FindControl("rptRentalCarMemberships");
            rpt.DataSource = ActiveGuestlist;
            rpt.DataBind();
        }



        private void dataBindFrequentFlyerTypes(System.Web.UI.WebControls.RepeaterItem ri)
        {
            if (_airLines.Count == 0)
            {
                _airLines.Add("Select an Option");
                _airLines.Add("Air Canada");
                _airLines.Add("Air France");
                _airLines.Add("Alaska Airlines");
                _airLines.Add("Alitalia");
                _airLines.Add("American");
                _airLines.Add("British Air");
                _airLines.Add("Cathay Pacific");
                _airLines.Add("Continental");
                _airLines.Add("Delta");
                _airLines.Add("Emirates Air");
                _airLines.Add("Frontier Air");
                _airLines.Add("Hawaiian Air");
                _airLines.Add("Japan Air");
                _airLines.Add("Jet Blue Airways");
                _airLines.Add("KLM Royal Dutch");
                _airLines.Add("Lufthansa");
                _airLines.Add("Midwest Express");
                _airLines.Add("Northwest");
                _airLines.Add("Qantas");
                _airLines.Add("Southwest");
                _airLines.Add("Swiss Airlines");
                _airLines.Add("United");
                _airLines.Add("US Airways");

            }
            List<string> FreqFlyerAirlineCodes = null;
            DropDownList ddl = (DropDownList)ri.FindControl("ddlFrequentFlyerNumber");

            if (FreqFlyerAirlineCodes == null)
            {
                FreqFlyerAirlineCodes = _airLines;

            }
            ddl.DataSource = FreqFlyerAirlineCodes;
            ddl.DataBind();
        }
        private void dataBindHotelClubMembershipsNumber(System.Web.UI.WebControls.RepeaterItem ri)
        {
            if (hotels.Count == 0)
            {
                hotels.Add("Select an Option");
                hotels.Add("Best Western");
                hotels.Add("Doubletree");
                hotels.Add("Fairfield");
                hotels.Add("Hampton");
                hotels.Add("Hilton");
                hotels.Add("Holiday Inn");
                hotels.Add("Hyatt");
                hotels.Add("La Quinta");
                hotels.Add("Marriott");
                hotels.Add("Omni");
                hotels.Add("Ramada");
                //hotels.Add("Other");
            }

            List<string> HotelMembership = null;
            DropDownList ddl = (DropDownList)ri.FindControl("ddlHotelClubMemberships");


            if (HotelMembership == null)
            {
                HotelMembership = hotels;

            }




            ddl.DataSource = HotelMembership;
            ddl.DataBind();
        }



        private void dataRentalCarMemberships(System.Web.UI.WebControls.RepeaterItem ri)
        {
            if (rentalcars.Count == 0)
            {
                rentalcars.Add("Select an Option");
                rentalcars.Add("Alamo");
                rentalcars.Add("Avis");
                rentalcars.Add("Budget");
                rentalcars.Add("Enterprise");
                rentalcars.Add("Hertz");
                rentalcars.Add("National");

            }

            List<string> RentalcarsMembership = null;
            DropDownList ddl = (DropDownList)ri.FindControl("ddlRentalCarMemberships");

            if (RentalcarsMembership == null)
            {
                RentalcarsMembership = rentalcars;

            }
            ddl.DataSource = RentalcarsMembership;
            ddl.DataBind();
        }

        public void SendTravelerProfileEmail()
        {

            string emailTemplate = Utilities.ReadCustomConfigValue("TravelerProfilerTemplate");
            StringBuilder emailTo = new StringBuilder(ConfigurationManager.AppSettings["ToEmail"]);
            StringBuilder emailFrom = new StringBuilder(Utilities.ReadCustomConfigValue("FromEmail"));
            StringBuilder emailSubject = new StringBuilder(ConfigurationManager.AppSettings["Subject"]);

            formatAndSendThirdPartyEmail(emailTemplate, emailTo, emailFrom, emailSubject);
        }
        private void formatAndSendThirdPartyEmail(string emailTemplate, StringBuilder emailTo, StringBuilder emailFrom, StringBuilder emailSubject)
        {
            CustomEmails email = new CustomEmails();

            StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

            // Replace any constants in the body of the email
            emailBody.Replace("[TravelerFirstName]", TravelerFirstName);
            emailBody.Replace("[TravelerMiddleName]", !Utilities.IsNothingNullOrEmpty(TravelerMiddleName) ? TravelerMiddleName : string.Empty);
            emailBody.Replace("[TravelerLastName]", TravelerLastName);
            emailBody.Replace("[CompanyName]", CompanyName);
            emailBody.Replace("[TravelerTitle]", TravelerTitle);
            emailBody.Replace("[DeptCostCenter]", !Utilities.IsNothingNullOrEmpty(DeptCostCenter) ? DeptCostCenter : string.Empty);
            emailBody.Replace("[EmailAddress]", EmailAddress);
            emailBody.Replace("[BusinessPhone]", BusinessPhone);
            emailBody.Replace("[BusinessFax]", BusinessFax);
            emailBody.Replace("[MobilePhone]", MobilePhone);
            emailBody.Replace("[HomePhone]", HomePhone);
            emailBody.Replace("[BirthDate]", BirthDate);
            emailBody.Replace("[Gender]", Gender);

            //Passport Information
            emailBody.Replace("[PassportName]", !Utilities.IsNothingNullOrEmpty(PassportName) ? PassportName : string.Empty);
            emailBody.Replace("[PassportNumber]", !Utilities.IsNothingNullOrEmpty(PassportNumber) ? PassportNumber : string.Empty);
            emailBody.Replace("[PassportIssueDate]", !Utilities.IsNothingNullOrEmpty(PassportIssueDate) ? PassportIssueDate : string.Empty);
            emailBody.Replace("[PassportExpirationDate]", !Utilities.IsNothingNullOrEmpty(PassportExpirationDate) ? PassportExpirationDate : string.Empty);
            emailBody.Replace("[PlaceofIssue]", !Utilities.IsNothingNullOrEmpty(PlaceofIssue) ? PlaceofIssue : string.Empty);

            //Traveler Arranger Information
            emailBody.Replace("[TravelerArrangerName]", !Utilities.IsNothingNullOrEmpty(TravelerArrangerName) ? TravelerArrangerName : string.Empty);
            emailBody.Replace("[TravelerArrangerPhone]", !Utilities.IsNothingNullOrEmpty(TravelerArrangerPhone) ? TravelerArrangerPhone : string.Empty);
            emailBody.Replace("[TravelerArrangerEmail]", !Utilities.IsNothingNullOrEmpty(TravelerArrangerEmail) ? TravelerArrangerEmail : string.Empty);

            //Emergency Contact Information
            emailBody.Replace("[EmergencyContactName]", EmergencyContactName);
            emailBody.Replace("[EmergencyContactRelationship]", EmergencyContactRelationship);
            emailBody.Replace("[EmergencyContactPhone]", EmergencyContactPhone);

            //Travel Preferences
            emailBody.Replace("[PreferredDepartureAirport]", PreferredDepartureAirport);
            emailBody.Replace("[PreferredCarrier]", !Utilities.IsNothingNullOrEmpty(PreferredCarrier) ? PreferredCarrier : string.Empty);
            emailBody.Replace("[OtherPreferredCarrier]", !Utilities.IsNothingNullOrEmpty(OtherPreferredCarrier) ? OtherPreferredCarrier : string.Empty);

            emailBody.Replace("[SeatingPreference]", SeatingPreference);
            emailBody.Replace("[SpecialMealRequirements]", !Utilities.IsNothingNullOrEmpty(SpecialMealRequirements) ? SpecialMealRequirements : string.Empty);

            //FrequentFlyerBlock
            FrequentFlyerBlock(emailBody);


            //Hotel Preferences
            emailBody.Replace("[SmokingPreference]", SmokingPreference);
            emailBody.Replace("[BedPreference]", BedPreference);
            emailBody.Replace("[SpecialRequirements]", !Utilities.IsNothingNullOrEmpty(SpecialRequirements) ? SpecialRequirements : string.Empty);

            //Hotel Membership block
            HotelClubMembershipsBlock(emailBody);

            if (!Utilities.IsNothingNullOrEmpty(txtOtherHotelMemberShip.Text))
            {
                emailBody.Replace("[OtherHotelMembership]", txtOtherHotelMemberShip.Text);
            }
            else
            {
                emailBody.Replace("[OtherHotelMembership]", txtOtherHotelMemberShip.Text);
            }

            if (!Utilities.IsNothingNullOrEmpty(txtOtherHotelClubMembershipsNumber.Text))
            {
                emailBody.Replace("[OtherHotelMembershipNumber]", txtOtherHotelClubMembershipsNumber.Text);
            }
            else
            {
                emailBody.Replace("[OtherHotelMembershipNumber]", txtOtherHotelClubMembershipsNumber.Text);
            }
                        

            //Rental Car Preferences
            emailBody.Replace("[VehicleSize]", VehicleSize);
            RentalcarBlock(emailBody);

            // Send the email (HTML)
            email.SendEmail(emailTo.ToString(), string.Empty, emailFrom.ToString(), emailSubject.ToString(), emailBody.ToString(), Enumerations.enumBodyPartFormats.formatHTML);
        }

        private void HotelClubMembershipsBlock(StringBuilder emailBody)
        {
            int startsAt = emailBody.ToString().IndexOf("<hotelclubmembershipsti>");
            int endsAt = emailBody.ToString().IndexOf("</hotelclubmembershipsti>");

            if (startsAt > 0 && endsAt > 0)
            {
                string template = emailBody.ToString()
                    .Substring(startsAt + "<hotelclubmembershipsti>".Length,
                        endsAt - startsAt - "</hotelclubmembershipsti>".Length +
                        ("</hotelclubmembershipsti>".Length - "<hotelclubmembershipsti>".Length));
                StringBuilder hotelmembershipBlock = new StringBuilder();
                StringBuilder loopTemplate = new StringBuilder();

                hotelmembershipBlock.Length = 0;

                if (template.Length > 0)
                {
                    //while (template.EndsWith("\r\n"))
                    //{
                    template = template.Substring(0, template.Length - 2);

                    foreach (HotelMembershipClass hotelMembership in HotelClubMemberships)
                    {
                        loopTemplate.Length = 0;
                        loopTemplate.Append(template);
                        loopTemplate.Replace("<<hotelclubmemberships>>", hotelMembership.HotelName);
                        loopTemplate.Replace("<<hotelclubmembershipsid>>", hotelMembership.HotelId);
                       
                        

                        hotelmembershipBlock.Append(loopTemplate.ToString());
                    }
                    emailBody.Replace(template, hotelmembershipBlock.ToString());
                    //}
                }
            }
        }

        private void FrequentFlyerBlock(StringBuilder emailBody)
        {
            int startsAt = emailBody.ToString().IndexOf("<frequentflyerti>");
            int endsAt = emailBody.ToString().IndexOf("</frequentflyerti>");

            if (startsAt > 0 && endsAt > 0)
            {
                string template = emailBody.ToString()
                    .Substring(startsAt + "<frequentflyerti>".Length,
                        endsAt - startsAt - "</frequentflyerti>".Length +
                        ("</frequentflyerti>".Length - "<frequentflyerti>".Length));
                StringBuilder frequentflyernumberBlock = new StringBuilder();
                StringBuilder loopTemplate = new StringBuilder();

                frequentflyernumberBlock.Length = 0;

                if (template.Length > 0)
                {
                    //while (template.EndsWith("\r\n"))
                    //{
                    template = template.Substring(0, template.Length - 2);

                    foreach (FrequentFlyerNumberClass frequentFlyerNumber in FrequentFlyerNumber)
                    {
                        loopTemplate.Length = 0;
                        loopTemplate.Append(template);
                        loopTemplate.Replace("<<frequentflyernumber>>", frequentFlyerNumber.FrequentFlyerNumber .ToString());
                        loopTemplate.Replace("<<frequentflyernumberid>>", frequentFlyerNumber.FrequentFlyerNumberId.ToString());

                        frequentflyernumberBlock.Append(loopTemplate.ToString());
                    }
                    emailBody.Replace(template, frequentflyernumberBlock.ToString());
                    //}
                }
            }
        }

        private void RentalcarBlock(StringBuilder emailBody)
        {
            int startsAt = emailBody.ToString().IndexOf("<rentalcarmembershipsti>");
            int endsAt = emailBody.ToString().IndexOf("</rentalcarmembershipsti>");

            if (startsAt > 0 && endsAt > 0)
            {
                string template = emailBody.ToString()
                    .Substring(startsAt + "<rentalcarmembershipsti>".Length,
                        endsAt - startsAt - "</rentalcarmembershipsti>".Length +
                        ("</rentalcarmembershipsti>".Length - "<rentalcarmembershipsti>".Length));
                StringBuilder rentalcarmembershipBlock = new StringBuilder();
                StringBuilder loopTemplate = new StringBuilder();

                rentalcarmembershipBlock.Length = 0;

                if (template.Length > 0)
                {
                    //while (template.EndsWith("\r\n"))
                    //{
                    template = template.Substring(0, template.Length - 2);

                    foreach (RentalCarClass rentalCarMembership in RentalCar)
                    {
                        loopTemplate.Length = 0;
                        loopTemplate.Append(template);
                        loopTemplate.Replace("<<rentalcarmemberships>>", rentalCarMembership.RentalCarName);
                        loopTemplate.Replace("<<rentalcarmembershipsid>>", rentalCarMembership.RentalCarId);

                        rentalcarmembershipBlock.Append(loopTemplate.ToString());
                    }
                    emailBody.Replace(template, rentalcarmembershipBlock.ToString());
                    //}
                }
            }
        }


        #endregion

        #region " ### Private Methods ### "

        #endregion

        #region " ### Public Overridden Methods ### "

        public override Label UserMessages
        {
            get { return this.lblInfo; }
            set { this.lblInfo.Text = value.Text; }
        }

        #endregion


    }


}