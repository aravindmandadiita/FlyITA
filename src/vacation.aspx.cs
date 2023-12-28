using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ITALib;
using PCentralLib;

namespace FlyITA
{
    public partial class vacation : PageBase
    {


        #region " ### Page Events ### "

        readonly List<string> _airLines = new List<string>();
        readonly List<string> hotels = new List<string>();
        readonly List<string> rentalcars = new List<string>();
        public List<PassangerInfoClass> Passengerdata = new List<PassangerInfoClass>();
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

                rptPersonInfo.DataSource = ActiveGuestlist;
                rptPersonInfo.DataBind();

            }

            foreach (RepeaterItem riPerson in this.rptPersonInfo.Items)
            {
                foreach (RepeaterItem riPassengerinfo in ((Repeater)riPerson.FindControl("rptPassangerInfo")).Items)
                {
                    ((Label)riPassengerinfo.FindControl("lblPassengerRequired")).Text = string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(), " First Name");
                    ((Label)riPassengerinfo.FindControl("lblPassengerMiddleName")).Text = string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(), " Middle Name");
                    ((Label)riPassengerinfo.FindControl("lblPassengerNameRequired")).Text = string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(), " Last Name");

                }
            }
        }


        protected void rptPerson_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindGuestData(e.Item);
        }
        protected void rptPersonInfo_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            dataBindPersonData(e.Item);
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
                if (ValidateInput(validation))
                {
                    if (!Utilities.IsNothingNullOrEmptyorZero(ContextManager.PassengerCount))
                    {
                        List<string> ActiveGuestlist = new List<string>();
                        for (int i = 1; i <= ContextManager.PassengerCount; i++)
                        {
                            ActiveGuestlist.Add(i.ToString());
                        }

                        rptPersonInfo.DataSource = ActiveGuestlist;
                        rptPersonInfo.DataBind();


                        foreach (RepeaterItem riPerson in this.rptPersonInfo.Items)
                        {

                            foreach (RepeaterItem riPassengerinfo in ((Repeater)riPerson.FindControl("rptPassangerInfo")).Items)
                            {
                                ((TextBox)riPassengerinfo.FindControl("txtPassengerFirstName")).Text =
                                    Passengerdata[riPerson.ItemIndex].pfirstname;
                                ((TextBox)riPassengerinfo.FindControl("txtPassengerMiddleName")).Text = Passengerdata[riPerson.ItemIndex].pmiddlename;
                                ((TextBox)riPassengerinfo.FindControl("txtPassengerLastName")).Text = Passengerdata[riPerson.ItemIndex].plastname;
                                ((TextBox)riPassengerinfo.FindControl("txtDateOfBirth")).Text = Passengerdata[riPerson.ItemIndex].pdob;
                                if (Utilities.IsNothingNullOrEmpty(Passengerdata[riPerson.ItemIndex].pgender))
                                {
                                    ((DropDownList)riPassengerinfo.FindControl("ddlGender")).SelectedIndex = 0;
                                }
                                else
                                {
                                    ((DropDownList)riPassengerinfo.FindControl("ddlGender")).SelectedItem.Text = Passengerdata[riPerson.ItemIndex].pgender;
                                }

                                ((TextBox)riPassengerinfo.FindControl("txtPassportNumber")).Text = Passengerdata[riPerson.ItemIndex].ppassportnumber; ;
                                ((TextBox)riPassengerinfo.FindControl("txtExpirationDate")).Text = Passengerdata[riPerson.ItemIndex].ppassportexp; ;
                                break;
                            }
                        }

                    }

                    foreach (RepeaterItem riPerson in this.rptPersonInfo.Items)
                    {
                        if (riPerson.ItemIndex!=ContextManager.PassengerCount-1)
                        {
                            (riPerson.FindControl("divselections")).Visible = false;
                        }

                        foreach (RepeaterItem riPassengerinfo in ((Repeater)riPerson.FindControl("rptPassangerInfo")).Items)
                        {
                            if (riPerson.ItemIndex == 0)
                            {
                                ((Label)riPassengerinfo.FindControl("lblPassengerRequired")).Text =
                                    string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(),
                                        " First Name");
                                ((Label)riPassengerinfo.FindControl("lblPassengerMiddleName")).Text =
                                    string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(),
                                        " Middle Name");
                                ((Label)riPassengerinfo.FindControl("lblPassengerNameRequired")).Text =
                                    string.Concat("Passenger ", (riPassengerinfo.ItemIndex + 1).ToString(), " Last Name");
                            }
                            else
                            {
                                ((Label)riPassengerinfo.FindControl("lblPassengerRequired")).Text =
                                    string.Concat("Passenger ", (riPerson.ItemIndex + riPassengerinfo.ItemIndex + 1).ToString(),
                                        " First Name");
                                ((Label)riPassengerinfo.FindControl("lblPassengerMiddleName")).Text =
                                    string.Concat("Passenger ", (riPerson.ItemIndex + riPassengerinfo.ItemIndex + 1).ToString(),
                                        " Middle Name");
                                ((Label)riPassengerinfo.FindControl("lblPassengerNameRequired")).Text =
                                    string.Concat("Passenger ", (riPerson.ItemIndex + riPassengerinfo.ItemIndex + 1).ToString(), " Last Name");

                            }
                        }
                    }
                    return;
                }

                SendTravelerProfileEmail();
                Utilities.Navigate("Thankyou.aspx?page=Vacation");
                ContextManager.PassengerCount = 0;
            }
            catch (Exception ex)
            {
                UserMessages.Text = ex.Message;
                throw;
            }


        }

        private bool ValidateInput(PCentralValidationResults validation)
        {
            //if (!BirthDate.isDateTime())
            //{
            //    validation.add_warning("Please enter a valid Date of Birth");
            //}
            //else if (BirthDate.toDateTime() > DateTime.Today)
            //{
            //    validation.add_warning("Date of Birth must be less than today's date");
            //}

            //if (!Utilities.IsNothingNullOrEmpty(PassportExpirationDate) && !PassportExpirationDate.isDateTime())
            //{
            //    validation.add_warning("Please enter a valid Passport Expiration Date");
            //}


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
            ContextManager.PassengerCount = 0;
            Passengerdata = null;
            foreach (PassangerInfoClass info in PassangerInfo)
            {
                ContextManager.PassengerCount++;
                if (!Utilities.IsNothingNullOrEmpty(info.pfirstname))
                {
                    if (Utilities.IsNothingNullOrEmpty(info.plastname))
                    {
                        validation.add_warning(string.Concat("Passenger Last Name required for ", info.pfirstname));
                    }

                    if (!Utilities.IsNothingNullOrEmpty(info.pdob))
                    {
                        if (!info.pdob.isDateTime())
                        {
                            validation.add_warning(string.Concat("Please enter a valid Date of Birth for ", info.pfirstname));
                        }
                        else if (info.pdob.toDateTime() > DateTime.Today)
                        {
                            validation.add_warning(string.Concat("Date of Birth must be less than today's date for ", info.pfirstname));
                        }
                    }
                    else
                    {
                        validation.add_warning(string.Concat("Date of Birth required for ", info.pfirstname));
                    }

                    if (Utilities.IsNothingNullOrEmpty(info.pgender))
                    {
                        validation.add_warning(string.Concat("Gender required for ", info.pfirstname));
                    }

                    if (!Utilities.IsNothingNullOrEmpty(info.ppassportexp) && !info.ppassportexp.isDateTime())
                    {
                        validation.add_warning(string.Concat("Please enter a valid Passport Expiration Date for ", info.pfirstname));
                    }

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

        public string NameofPersonRequesting
        {
            get { return txtNameofPersonRequesting.Text; }

        }
        public string GeneralandPassengerEmail
        {
            get { return txtGeneralandPassengerEmail.Text; }

        }
        public string PhoneNumber
        {
            get { return txtPhoneNumber.Text; }

        }
        public string PassengerFirstName
        {
            get { return this.txtPassengerFirstName.Text; }
        }

        public string PassengerLastName
        {
            get { return this.txtPassengerLastName.Text; }
        }
        public string PassengerMiddleName
        {
            get { return txtPassengerMiddleName.Text; }

        }



        public string BirthDate
        {
            get { return txtDateOfBirth.Text; }

        }
        public string Gender
        {
            get { return ddlGender.SelectedItem.Text; }

        }


        public string PassportNumber
        {
            get { return txtPassportNumber.Text; }

        }

        public string PassportExpirationDate
        {
            get { return txtExpirationDate.Text; }

        }
        public string Airline
        {
            get { return ddlAirline.SelectedItem.Text; }

        }

        public string DepartureCity
        {
            get { return txtDepartureCity.Text; }

        }

        public string PreferredAirline
        {
            get { return ddlPreferredAirline.SelectedItem.Text; }

        }

        public string DestinationsInterestedIn
        {
            get { return txtDestinationsInterestedIn.Text; }

        }
        public string PreferredDatesofTravel
        {
            get { return txtPreferredDatesofTravel.Text; }

        }


        public string DestinationsNotInterestedIn
        {
            get { return txtDestinationsInterestedIn.Text; }

        }


        public string VacationDetails
        {
            get { return txtVacationDetails.Text; }

        }


        public string ImportantAmenities
        {
            get { return txtImportantAmenities.Text; }

        }

        public string RoomsNeeded
        {
            get { return ddlRoomsNeeded.SelectedItem.Text; }

        }

        public class FrequentFlyerNumberClass
        {
            public string FrequentFlyerNumber { get; set; }
            public string FrequentFlyerNumberId { get; set; }
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
                            ht.Add(new FrequentFlyerNumberClass { FrequentFlyerNumber = selectedFrequentFlyerNumber, FrequentFlyerNumberId = selectedFrequentFlyerNumberId });

                        }
                    }
                }

                return ht;
            }

        }
        public class PassangerInfoClass
        {
            public string pfirstname { get; set; }
            public string pmiddlename { get; set; }
            public string plastname { get; set; }
            public string pdob { get; set; }
            public string pgender { get; set; }
            public string ppassportnumber { get; set; }
            public string ppassportexp { get; set; }


        }
        public List<PassangerInfoClass> PassangerInfo
        {
            get
            {
                List<PassangerInfoClass> ht = new List<PassangerInfoClass>();


                foreach (RepeaterItem riPerson in this.rptPersonInfo.Items)
                {
                    foreach (RepeaterItem riPassengerinfo in ((Repeater)riPerson.FindControl("rptPassangerInfo")).Items)
                    {


                        string pfirstname = ((TextBox)riPassengerinfo.FindControl("txtPassengerFirstName")).Text;
                        string pmiddlename = ((TextBox)riPassengerinfo.FindControl("txtPassengerMiddleName")).Text;
                        string plastname = ((TextBox)riPassengerinfo.FindControl("txtPassengerLastName")).Text;
                        string pdob = ((TextBox)riPassengerinfo.FindControl("txtDateOfBirth")).Text;
                        string pgender = ((DropDownList)riPassengerinfo.FindControl("ddlGender")).SelectedItem.Text.Replace("Select an Option", string.Empty);
                        string ppassportnumber = ((TextBox)riPassengerinfo.FindControl("txtPassportNumber")).Text;
                        string ppassportexp = ((TextBox)riPassengerinfo.FindControl("txtExpirationDate")).Text;
                        if (!pfirstname.IsNullOrWhiteSpace())
                            ht.Add(new PassangerInfoClass { pfirstname = pfirstname, pmiddlename = pmiddlename, plastname = plastname, pdob = pdob, pgender = pgender, ppassportnumber = ppassportnumber, ppassportexp = ppassportexp });


                    }
                }
                Passengerdata = ht;
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
        protected void rptPassangerInfo_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {

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

        private void dataBindPersonData(System.Web.UI.WebControls.RepeaterItem ri)
        {
            Repeater rpt;
            List<string> ActiveGuestlist = new List<string>();

            if (!Utilities.IsNothingNullOrEmptyorZero(ContextManager.PassengerCount))
            {

                for (int i = 6; i >= ContextManager.PassengerCount; i--)
                {
                    ActiveGuestlist.Add(i.ToString());
                }
            }
            else { 
            ActiveGuestlist.Add("1");
            ActiveGuestlist.Add("2");
            ActiveGuestlist.Add("3");
            ActiveGuestlist.Add("4");
            ActiveGuestlist.Add("5");
            ActiveGuestlist.Add("6");
            }
            rpt = (Repeater)ri.FindControl("rptPassangerInfo");
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


        public void SendTravelerProfileEmail()
        {

            string emailTemplate = Utilities.ReadCustomConfigValue("VacationTravelRequesTemplate");
            StringBuilder emailTo = new StringBuilder(ConfigurationManager.AppSettings["VacationToEmail"]);
            StringBuilder emailFrom = new StringBuilder(Utilities.ReadCustomConfigValue("FromEmail"));
            StringBuilder emailSubject = new StringBuilder(ConfigurationManager.AppSettings["VacationSubject"]);

            formatAndSendThirdPartyEmail(emailTemplate, emailTo, emailFrom, emailSubject);
        }
        private void formatAndSendThirdPartyEmail(string emailTemplate, StringBuilder emailTo, StringBuilder emailFrom, StringBuilder emailSubject)
        {
            CustomEmails email = new CustomEmails();

            StringBuilder emailBody = Utilities.LoadFileContent(emailTemplate);

            // Replace any constants in the body of the email
            emailBody.Replace("[NameofPersonRequesting]", NameofPersonRequesting);
            emailBody.Replace("[GeneralandPassengerEmail]", GeneralandPassengerEmail);
            emailBody.Replace("[PhoneNumber]", PhoneNumber);
            emailBody.Replace("[PassengerFirstName]", PassengerFirstName);
            emailBody.Replace("[PassengerLastName]", PassengerLastName);
            emailBody.Replace("[BirthDate]", BirthDate);
            emailBody.Replace("[Gender]", Gender);
            emailBody.Replace("[PassportNumber]", PassportNumber);
            emailBody.Replace("[PassportExpirationDate]", PassportExpirationDate);

            emailBody.Replace("[Airline]", Airline);
            //FrequentFlyerBlock
            PassengerInfoBlock(emailBody);
            FrequentFlyerBlock(emailBody);


            //Trip Information
            emailBody.Replace("[DepartureCity]", DepartureCity);
            emailBody.Replace("[PreferredAirline]", PreferredAirline);
            emailBody.Replace("[DestinationsInterestedIn]", DestinationsInterestedIn);
            emailBody.Replace("[PreferredDatesofTravel]", PreferredDatesofTravel);
            emailBody.Replace("[DestinationsNotInterestedIn]", DestinationsNotInterestedIn);
            emailBody.Replace("[VacationDetails]", VacationDetails);
            emailBody.Replace("[ImportantAmenities]", ImportantAmenities);
            emailBody.Replace("[RoomsNeeded]", RoomsNeeded);




            // Send the email (HTML)
            email.SendEmail(emailTo.ToString(), string.Empty, emailFrom.ToString(), emailSubject.ToString(), emailBody.ToString(), Enumerations.enumBodyPartFormats.formatHTML);
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
                        loopTemplate.Replace("<<frequentflyernumber>>", frequentFlyerNumber.FrequentFlyerNumber.ToString());
                        loopTemplate.Replace("<<frequentflyernumberid>>", frequentFlyerNumber.FrequentFlyerNumberId.ToString());

                        frequentflyernumberBlock.Append(loopTemplate.ToString());
                    }
                    emailBody.Replace(template, frequentflyernumberBlock.ToString());
                    //}
                }
            }
        }

        private void PassengerInfoBlock(StringBuilder emailBody)
        {
            int startsAt = emailBody.ToString().IndexOf("<passengerinfoi>");
            int endsAt = emailBody.ToString().IndexOf("</passengerinfoi>");

            if (startsAt > 0 && endsAt > 0)
            {
                string template = emailBody.ToString()
                    .Substring(startsAt + "<passengerinfoi>".Length,
                        endsAt - startsAt - "</passengerinfoi>".Length +
                        ("</passengerinfoi>".Length - "<passengerinfoi>".Length));
                StringBuilder passangerinfoBlock = new StringBuilder();
                StringBuilder loopTemplate = new StringBuilder();

                passangerinfoBlock.Length = 0;

                if (template.Length > 0)
                {
                    //while (template.EndsWith("\r\n"))
                    //{
                    template = template.Substring(0, template.Length - 2);
                    int count = 0;
                    foreach (PassangerInfoClass info in PassangerInfo)
                    {
                        count++;
                        loopTemplate.Length = 0;
                        loopTemplate.Append(template);
                        loopTemplate.Replace("<<pfirstname>>", info.pfirstname.ToString());
                        loopTemplate.Replace("<<pmiddlename>>", info.pmiddlename.ToString());
                        loopTemplate.Replace("<<plastname>>", info.plastname.ToString());
                        loopTemplate.Replace("<<pdob>>", info.pdob.ToString());
                        loopTemplate.Replace("<<pgender>>", info.pgender.ToString());
                        loopTemplate.Replace("<<ppassportnumber>>", info.ppassportnumber.ToString());
                        loopTemplate.Replace("<<ppassportexp>>", info.ppassportexp.ToString());
                        loopTemplate.Replace("Passenger First Name", string.Concat("Passenger ", count.ToString(), " First Name"));
                        loopTemplate.Replace("Passenger Middle Name", string.Concat("Passenger ", count.ToString(), " Middle Name"));
                        loopTemplate.Replace("Passenger Last Name", string.Concat("Passenger ", count.ToString(), " Last Name"));
                        passangerinfoBlock.Append(loopTemplate.ToString());
                    }
                    emailBody.Replace(template, passangerinfoBlock.ToString());
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