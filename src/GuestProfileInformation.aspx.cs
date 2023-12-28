using System.Text;
using System.Web.UI.WebControls;
using PCentralLib;
using System;
using System.Collections;
using ITALib;

namespace FlyITA
{
    public partial class GuestProfileInformation : PageBase, IGuestProfileView
    {
       

        private GuestProfilePresenter _Presenter;

        protected void Page_PreRender(System.Object sender, System.EventArgs e)
        {
            //if (ddlProofofCitizenshipType.SelectedValue.toInt().IsOneOf(
            //    (int) Enumerations.enumProofofCitizenshipTypes.PhotoID,
            //    (int) Enumerations.enumProofofCitizenshipTypes.CertifiedCopyBirthCertificate)) {
            //    pnlPassport.CssClass = "hidden";
            //    rbPassportStatusNo.Checked = false;
            //    rbPassportStatusYes.Checked = false;
            //} else if (ddlProofofCitizenshipType.SelectedValue.toInt() == (int) Enumerations.enumProofofCitizenshipTypes.Passport) {
            //    pnlPassport.CssClass = "show";
            //}
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _Presenter = new GuestProfilePresenter(this);
            //this is needed to re-draw the CF controls on the page, if they are dynamically added
            //AddCustomControl();
            // set ParticipantID's here
            //_Presenter.PreInit();
            if (!IsPostBack)
            {
                //_Presenter.Init();
                //setupProofOfCitizenshipTypes()
                //setDisplayFromConfig()
            }



            //if (this.divLegalName.Visible == true)
            //{
            //    Utilities.SetFocus(this.ddlLegalPrefix);
            //}
            //else if (this.divNickName.Visible == true)
            //{
            //    Utilities.SetFocus(this.txtNickname);
            //}
            //else if (this.divBadgeName.Visible == true)
            //{
            //    Utilities.SetFocus(this.txtBadgeFirstName);
            //}
            //else if (this.divDisplayDOB.Visible == true)
            //{
            //    Utilities.SetFocus(this.txtDateOfBirth);
            //}
            //else if (this.divDisplayGender.Visible == true)
            //{
            //    Utilities.SetFocus(this.ddlGender);
            //}
            //else
            //{
            //    Utilities.SetFocus(this.ddlProofofCitizenshipType);
            //}

            //EnableDisableOtherDietaryPreference();
            //EnableDisableOtherSpecificAids();
        }
        private void EnableDisableOtherDietaryPreference()
        {
            foreach (ListItem listitem in cblSpecialDiet.Items)
            {
                if (listitem.Value == "Other" && listitem.Selected)
                {
                    pnlOtherDiet.Visible = listitem.Selected;
                    txtOtherDietaryPreferences.Attributes["required"] = "required";
                }
                else if (listitem.Value == "Other" && !listitem.Selected)
                {
                    pnlOtherDiet.Visible = listitem.Selected;
                    txtOtherDietaryPreferences.Attributes.Remove("required");
                    txtOtherDietaryPreferences.Text = string.Empty;
                }
            }
        }
        private void EnableDisableOtherSpecificAids()
        {

            foreach (ListItem listitem in chkSpecificAids.Items)
            {
                if (listitem.Value == "Other" && listitem.Selected)
                {
                    pnlOtherSpecificAids.Visible = listitem.Selected;
                    txtOtherSpecificAids.Attributes["required"] = "required";
                }
                else if (listitem.Value == "Other" && !listitem.Selected)
                {
                    pnlOtherSpecificAids.Visible = listitem.Selected;
                    txtOtherSpecificAids.Attributes.Remove("required");
                    txtOtherSpecificAids.Text = string.Empty;
                }
            }
        }
        #region " ### Model Properties ### "

        public System.Data.DataTable PrefixList
        {
            set
            {
                if (EditableLegalNameVisible && PrefixVisible)
                {
                    Utilities.LoadList(ctl: this.ddlLegalPrefix, source: ref value, DefaultText: "Select an Option");
                }
            }
        }

        public System.Data.DataTable SuffixList
        {
            set
            {
                if (EditableLegalNameVisible && SuffixVisible)
                {

                    Utilities.LoadList(ctl: this.ddlLegalSuffix, source: ref value, DefaultText: "Select an Option");
                }
                Utilities.LoadList(ctl: this.ddlSuffixPassport, source: ref value, DefaultText: "Select an Option");
            }
        }

        public System.Data.DataTable CountryList
        {
            set
            {
                Utilities.LoadList(ctl: this.ddlCountryOfIssue, source: ref value, DefaultText: "Select an Option", CheckNoDefault: true);
                Utilities.LoadList(ctl: this.ddlNationality, source: ref value, DefaultText: "Select an Option", CheckNoDefault: true);
                //If BirthCertificateVisible Then
                //    Utilities.LoadList(Me.ddlBirthCertificate, value)
                //End If
            }
        }
        public string LegalNameVerbiage
        {
            get { return this.lblLegalNameVerbiage.Text; }
            set { this.lblLegalNameVerbiage.Text = value; }
        }
        public bool LegalNameVerbiageVisible
        {
            get { return this.lblLegalNameVerbiage.Visible; }
            set
            {
                this.lblLegalNameVerbiage.Visible = value;
                this.paragraphLegalNameVerbiage.Visible = value;
            }
        }

        public System.Data.DataTable GenderList
        {
            set { Utilities.LoadList(ctl: this.ddlGender, source: ref value, DefaultText: "Select an Option"); }
        }

        public System.Data.DataTable ProofofCitizenshipTypeList
        {
            set
            {
                //ms DHS rule apply
                //taking out Birth Certificate, this catch it to catch anyone coming back with Birth Cert saved previously
                try
                {
                    Utilities.LoadList(ctl: this.ddlProofofCitizenshipType, source: ref value, DefaultText: "Select an Option");
                }
                catch
                {
                    //do nothing, force the user to reselect valid value
                }
                //ms end DHS rule
                //Utilities.LoadList(Me.ddlProofofCitizenshipType, value, , , "Select")
            }
        }

        public string BadgeFirstName
        {
            get { return this.txtBadgeFirstName.Text; }
            set { this.txtBadgeFirstName.Text = value; }
        }

        public string BadgeLastName
        {
            get { return this.txtBadgeLastName.Text; }
            set { this.txtBadgeLastName.Text = value; }
        }

        public string DateOfBirth
        {
            get { return this.txtDateOfBirth.Text; }
            set { this.txtDateOfBirth.Text = value; }
        }

        public string DietaryPreferences
        {
            get
            {
                StringBuilder sb = default(StringBuilder);

                sb = new StringBuilder();
                foreach (ListItem listitem in cblSpecialDiet.Items)
                {
                    if (listitem.Selected)
                    {
                        sb = sb.Append(listitem.Text);
                        sb = sb.Append(";");
                    }

                }

                if (!Utilities.IsNothingNullOrEmpty(txtOtherDietaryPreferences.Text))
                    sb.Append(":");

                sb.Append(this.txtOtherDietaryPreferences.Text.TrimEnd(':')).Replace("  ", "\r\n");
                return sb.ToString();


            }
            set
            {
                if (value != null)
                {


                    foreach (string s in value.Split(';'))
                    {
                        foreach (ListItem l in cblSpecialDiet.Items)
                        {
                            if (s == l.Text)
                            {
                                l.Selected = true;
                            }
                        }
                    }
                    if (value.Contains(":"))
                    {
                        this.txtOtherDietaryPreferences.Text = value.Split(':')[1].Replace("  ", "\r\n");
                    }
                }
            }
        }


        public string PassportExpirationDate
        {
            get { return this.txtExpirationDate.Text; }
            set { this.txtExpirationDate.Text = value; }
        }

        public string FirstPassportName
        {
            get { return this.txtFirstPassportName.Text; }
            set { this.txtFirstPassportName.Text = value; }
        }

        public string GetSetBirthCertificate
        {
            get { return this.ddlBirthCertificate.SelectedValue; }
            set { this.ddlBirthCertificate.SelectedValue = value; }
        }

        public string GetSetCountryOfIssue
        {
            get { return this.ddlCountryOfIssue.SelectedValue; }
            set { this.ddlCountryOfIssue.SelectedValue = value; }
        }

        public string GetSetGender
        {
            get { return this.ddlGender.SelectedValue; }
            set { this.ddlGender.SelectedValue = value; }
        }

        public string GetSetNationality
        {
            get { return this.ddlNationality.SelectedValue; }
            set { this.ddlNationality.SelectedValue = value; }
        }

        public Enumerations.enumProofofCitizenshipTypes GetSetProofofCitizenshipType
        {
            get
            {
                int selected;
                int.TryParse(ddlProofofCitizenshipType.SelectedValue, out selected);
                return (Enumerations.enumProofofCitizenshipTypes)selected;
            }
            set { ddlProofofCitizenshipType.SelectedValue = (Convert.ToInt32(value)).ToString(); }
        }

        public string GetSetSuffixPassport
        {
            get { return this.ddlSuffixPassport.SelectedValue; }
            set
            {
                string sfx = (value == "Jr" ? "Jr." : (value == "Sr" ? "Sr." : value));
                this.ddlSuffixPassport.SelectedValue = ddlSuffixPassport.Items.FindByText(sfx).Value;
            }
        }

        public string LastPassportName
        {
            get { return this.txtLastPassportName.Text; }
            set { this.txtLastPassportName.Text = value; }
        }

        public string LegalName
        {
            get { return this.lblName.Text; }
            set { this.lblName.Text = value; }
        }

        public string MiddlePassportName
        {
            get { return this.txtMiddlePassportName.Text; }
            set { this.txtMiddlePassportName.Text = value; }
        }

        public string Nickname
        {
            get { return this.txtNickname.Text; }
            set { this.txtNickname.Text = value; }
        }

        public string PassportNumber
        {
            get { return this.txtPassportNumber.Text; }
            set { this.txtPassportNumber.Text = value; }
        }

        //Public Property PassportStatusAvailable() As Boolean Implements IGuestProfileView.PassportStatusAvailable
        //	Get
        //		If Me.rbPassportStatusYes.Checked Then
        //			Return True
        //		ElseIf Me.rbPassportStatusNo.Checked Then
        //			Return False
        //		End If
        //	End Get
        //	Set(ByVal value As Boolean)
        //		If value Then
        //			Me.rbPassportStatusYes.Checked = True
        //		Else
        //			Me.rbPassportStatusNo.Checked = True
        //		End If
        //	End Set
        //End Property

        public enumTriBoolean HasPassportInformation
        {
            get
            {
                if (rbPassportStatusNo.Checked)
                {
                    return enumTriBoolean.tbFalse;
                }
                else if (rbPassportStatusYes.Checked)
                {
                    return enumTriBoolean.tbTrue;
                }
                else
                {
                    return enumTriBoolean.tbUnassigned;
                }
            }
            set
            {
                if (value == enumTriBoolean.tbTrue)
                {
                    rbPassportStatusNo.Checked = false;
                    rbPassportStatusYes.Checked = true;
                }
                else if (value == enumTriBoolean.tbFalse)
                {
                    rbPassportStatusNo.Checked = true;
                    rbPassportStatusYes.Checked = false;
                }
                else
                {
                    rbPassportStatusNo.Checked = false;
                    rbPassportStatusYes.Checked = false;
                }
            }
        }

        public string PhysicalAssistanceRequests
        {
            get
            {
                StringBuilder sb = default(StringBuilder);

                sb = new StringBuilder();
                foreach (ListItem listitem in chkSpecificAids.Items)
                {
                    if (listitem.Selected)
                    {
                        sb = sb.Append(listitem.Text);
                        sb = sb.Append(";");
                    }

                }

                if (!Utilities.IsNothingNullOrEmpty(txtOtherSpecificAids.Text))
                    sb.Append(":");

                sb.Append(this.txtOtherSpecificAids.Text.TrimEnd(':'));
                return sb.ToString();


            }
            set
            {
                if (value != null)
                {


                    foreach (string s in value.Split(';'))
                    {
                        foreach (ListItem l in chkSpecificAids.Items)
                        {
                            if (s == l.Text)
                            {
                                l.Selected = true;
                            }
                        }
                    }
                    if (value.Contains(":"))
                    {
                        this.txtOtherSpecificAids.Text = value.Split(':')[1].Replace("  ", "\r\n");
                    }
                }
            }
        }

        //Public Property IsPassportAvailableSelected() As Boolean Implements IGuestProfileView.IsPassportAvailableSelected
        //	Get
        //		If (Not Me.rbPassportStatusNo.Checked) AndAlso (Not Me.rbPassportStatusYes.Checked) Then
        //			Return False
        //		Else
        //			Return True
        //		End If
        //	End Get
        //	Set(ByVal value As Boolean)

        //	End Set
        //End Property

        public string CitizenshipTypeIfOnlyOneAvailable
        {
            get { return lblProofOfCitizenshipType.Text; }
            set { lblProofOfCitizenshipType.Text = value; }
        }

        public bool MoreThanOneCitizenshipTypeAvailable
        {
            get { return ddlProofofCitizenshipType.Visible; }
            set
            {
                ddlProofofCitizenshipType.Visible = value;
                lblProofOfCitizenshipType.Visible = !value;
                if (value)
                {
                    dvProofOfCitizenshipReq.Attributes["class"] = "form-group required";
                    ddlProofofCitizenshipType.Attributes["required"] = "required";
                    ddlProofofCitizenshipType.Visible = true;
                }
                else
                {
                    dvProofOfCitizenshipReq.Attributes["class"] = "form-group";
                    ddlProofofCitizenshipType.Attributes.Remove("required");
                }
            }
        }

        public string LegalPrefix
        {
            get { return this.ddlLegalPrefix.SelectedValue; }
            set { this.ddlLegalPrefix.SelectedValue = value; }
        }

        public string LegalSuffix
        {
            get { return this.ddlLegalSuffix.SelectedValue; }
            set { this.ddlLegalSuffix.SelectedValue = value; }
        }

        public string LegalFirstName
        {
            get { return this.txtLegalFirstName.Text; }
            set { this.txtLegalFirstName.Text = value; }
        }

        public string LegalMiddleName
        {
            get { return this.txtLegalMiddleName.Text; }
            set { this.txtLegalMiddleName.Text = value; }
        }

        public string LegalLastName
        {
            get { return this.txtLegalLastName.Text; }
            set { this.txtLegalLastName.Text = value; }
        }
        public string CurrentPage
        {
            get { return Utilities.GetCurrentPage().Split('.')[0]; }
        }


        public string PassportIssueDate
        {
            get { return txtPassportIssueDate.Text; }
            set { txtPassportIssueDate.Text = value; }
        }

        public string PassportIssuingAuthority
        {
            get { return txtPassportIssueAuthority.Text; }
            set { txtPassportIssueAuthority.Text = value; }
        }
        #endregion

        #region " ### Public Overridden Methods ### "

        public override System.Web.UI.WebControls.Label UserMessages
        {
            get { return this.lblInfo; }
            set { this.lblInfo.Text = value.Text; }
        }

        #endregion

        #region " DTO Properties "
        public bool PrefixVisible
        {
            get { return this.divddlPrefix.Visible; }
            set
            {
                this.lblPrefix.Visible = value;
                this.divddlPrefix.Visible = value;
                this.divNoPrefix.Visible = value;
            }
        }

        public bool SuffixVisible
        {
            get { return this.divddlSufix.Visible; }
            set
            {
                this.lblSufix.Visible = value;
                this.divddlSufix.Visible = value;
            }
        }
        public string PrefixText
        {
            set { this.lblPrefix.Text = value; }
        }

        public string SuffixText
        {
            set { this.lblSufix.Text = value; }
        }

        public string InformationText
        {
            set { }
        }

        public bool InformationTextVisible
        {
            set { }
        }

        public string WelcomeText
        {
            set { this.lblWelcomeText.Text = value; }
        }

        public bool WelcomeTextVisible
        {
            set { this.lblWelcomeText.Visible = value; }
        }

        public string GeneralInfoSectionHeadingText
        {
            set { this.lblGeneralInfoSectionHeading.Text = value; }
        }

        public bool GeneralInfoSectionVisible
        {
            get { return lblGeneralInfoSectionHeading.Visible; }
            set { lblGeneralInfoSectionHeading.Visible = value; }
        }

        public string NameSubSectionText
        {
            set { this.lblNameSubSection.Text = value; }
        }

        public bool NameSubSectionVisible
        {
            get { return this.lblNameSubSection.Visible; }
            set { this.lblNameSubSection.Visible = value; }
        }

        public string DisplayNameText
        {
            set { lblDisplayName.Text = value; }
        }

        public bool DisplayNameVisible
        {
            get { return lblDisplayName.Visible; }
            set { lblDisplayName.Visible = value; }
        }

        public string PhysicalReqText
        {
            set { this.lblPhysicalReqText.Text = value; }
        }

        public string SpecialReqSubSection
        {
            set { this.lblSpecialReqSubSection.Text = value; }
        }

        public bool SpecialRequestVisible
        {
            get { return pnlSpecialRequest.Visible; }
            set { this.pnlSpecialRequest.Visible = value; }
        }

        public bool ProofOfCitizenshipVisible
        {
            get
            {
                if (this.pnlProofOfCitizenship.Visible)
                    return true;
                if (this.pnlPassport.Visible)
                    return true;
                if (this.pnlBirthCertificate.Visible)
                    return true;
                return false;
            }
            set
            {
                this.pnlProofOfCitizenship.Visible = value;
                this.pnlPassport.Visible = value;
                this.pnlBirthCertificate.Visible = value;
            }
        }

        public string POCSectionHeader
        {
            set { this.lblPOCSectionHeader.Text = value; }
        }

        public string POCSubSection
        {
            set { this.lblPOCSubSection.Text = value; }
        }

        public bool POCRequired
        {
            get { return this.dvProofOfCitizenshipReq.Attributes["class"].Contains("required"); }
            set
            {
                if (value) return;
                this.dvProofOfCitizenshipReq.Attributes["class"] = "form-group";
                this.ddlProofofCitizenshipType.Attributes.Remove("required");
            }
        }

        public bool POCTypePassportOnly
        {
            get
            {
                if ((lblProofOfCitizenshipType.Visible == true && ddlProofofCitizenshipType.Visible == false))
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    dvProofOfCitizenshipReq.Attributes["class"] = "form-group";
                    this.ddlProofofCitizenshipType.Attributes.Remove("required");

                    //// Only one is visible - hide the DDL, replace with LBL
                    //ddlProofofCitizenshipType.SelectedValue = _
                    // ddlProofofCitizenshipType.Items.FindByText("Passport").Value

                    ddlProofofCitizenshipType.Visible = false;
                    lblProofOfCitizenshipType.Text = "Passport";
                    lblProofOfCitizenshipType.Visible = true;
                    //BirthCertificateVisible = False

                    //lblProofOfCitizenshipType.Visible = True
                    //ddlProofofCitizenshipType.Visible = False
                }
                else
                {
                    //check against the default
                    if (ddlProofofCitizenshipType.Visible == false)
                    {
                        dvProofOfCitizenshipReq.Attributes["class"] = "form-group required";
                        this.ddlProofofCitizenshipType.Attributes["required"] = "required";
                        ddlProofofCitizenshipType.Visible = true;
                        lblProofOfCitizenshipType.Text = "";
                        lblProofOfCitizenshipType.Visible = false;
                    }
                }
            }
        }

        public string POCTypeText
        {
            set { this.lblPOCTypeText.Text = value; }
        }

        public bool PassportVisible
        {
            get { return Convert.ToBoolean(pnlPassportVisibility.Visible); }
            set
            {
                this.pnlPassportVisibility.Value = value.ToString();
                if (value == false)
                {
                    dvProofOfCitizenshipReq.Attributes["class"] = "form-group";
                    this.ddlProofofCitizenshipType.Attributes.Remove("required");
                    //// Only one is visible - hide the DDL, replace with LBL
                    ddlProofofCitizenshipType.Visible = false;
                    lblProofOfCitizenshipType.Text = "Photo ID";
                    lblProofOfCitizenshipType.Visible = true;
                    //BirthCertificateVisible = True
                }
            }
        }

        public string PassportSubSection
        {
            set { this.lblPassportSubSection.Text = value; }
        }

        public string PassportAvailableText
        {
            set { this.lblPassportAvailableText.Text = value; }
        }

        public string PassportStatusNoText
        {
            set { lblPassportStatusNo.Text = value; }
        }

        public string PassportStatusYesText
        {
            set { lblPassportStatusYes.Text = value; }
        }

        public string PassportExpDateText
        {
            set { this.lblPassportExpDateText.Text = value; }
        }

        public string PassportFNameText
        {
            set { this.lblPassportFNameText.Text = value; }
        }

        public string PassportIssuedText
        {
            set { this.lblPassportIssuedText.Text = value; }
        }

        public string PassportLNameText
        {
            set { this.lblPassportLNameText.Text = value; }
        }

        public string PassportMNameText
        {
            set { this.lblPassportMNameText.Text = value; }
        }

        public string PassportNationText
        {
            set { this.lblPassportNationText.Text = value; }
        }

        public string PassportNbrText
        {
            set { this.lblPassportNbrText.Text = value; }
        }

        public string PassportSuffixText
        {
            set { this.lblPassportSuffixText.Text = value; }
        }

        public bool BirthCertificateVisible
        {
            get { return this.pnlBirthCertificate.Visible; }
            set { this.pnlBirthCertificate.Visible = value; }
        }

        public string BirthCSubSection
        {
            set { this.lblBirthCSubSection.Text = value; }
        }

        public string BCertNationText
        {
            set { this.lblBCertNationText.Text = value; }
        }

        public string DietPrefText
        {
            set { this.lblDietPrefText.Text = value; }
        }

        public string PersonalInfoHeaderText
        {
            set { this.lblPersonalInfoSubSectionText.Text = value; }
        }

        public bool PersonalInfoSectionVisible
        {
            get { return this.pnlDisplayPersonalInfoHeader.Visible; }
            set { this.pnlDisplayPersonalInfoHeader.Visible = value; }
        }

        public bool DOBRequired
        {
            get { return this.dvDOBRequired.Attributes["class"].Contains("required"); }
            set
            {
                if (value) return;
                this.dvDOBRequired.Attributes["class"] = "form-group";
                this.txtDateOfBirth.Attributes.Remove("required");
            }
        }

        public string DOBText
        {
            set { this.lblDOBText.Text = value; }
        }

        public bool DOBVisible
        {
            get { return this.divDisplayDOB.Visible; }
            set { this.divDisplayDOB.Visible = value; }
        }

        public bool GenderRequired
        {
            get { return this.dvGenderRequired.Attributes["class"].Contains("required"); }
            set
            {
                if (value) return;
                this.dvGenderRequired.Attributes["class"] = "form-group";
                this.ddlGender.Attributes.Remove("required");
            }
        }

        public string GenderText
        {
            set { this.lblGenderText.Text = value; }
        }

        public bool GenderVisible
        {
            get { return this.divDisplayGender.Visible; }
            set { this.divDisplayGender.Visible = value; }
        }

        public bool ValidatePassportName
        {
            get
            {
                return lblDisplayName.Text == "Passport Name";
            }
            set
            {
                if (value)
                {
                    lblDisplayName.Text = "Passport Name";
                    lblLegalName.Text = "Not your Passport Name?";
                }
            }
        }

        public bool NickNameVisible
        {
            get { return this.divNickName.Visible; }
            set { this.divNickName.Visible = value; }
        }

        public string NickNameText
        {
            get { return lblNicknameText.Text; }
            set { this.lblNicknameText.Text = value; }
        }

        public bool BadgeFNameRequired
        {
            get { return this.dvBadgeFirstRequired.Attributes["class"].Contains("required"); }
            set
            {
                if (value) return;
                this.dvBadgeFirstRequired.Attributes["class"] = "form-group";
                this.txtBadgeFirstName.Attributes.Remove("required");
            }
        }

        public string BadgeFNameText
        {
            set { this.lblBadgeFNameText.Text = value; }
        }

        public bool BadgeNameVisible
        {
            get { return this.divBadgeName.Visible; }
            set { this.divBadgeName.Visible = value; }
        }

        public bool BadgeLNameRequired
        {
            get { return this.dvBadgeLastRequired.Attributes["class"].Contains("required"); }
            set
            {
                if (value) return;
                this.dvBadgeLastRequired.Attributes["class"] = "form-group";
                this.txtBadgeLastName.Attributes.Remove("required");
            }
        }

        public string BadgeLNameText
        {
            set { this.lblBadgeLNameText.Text = value; }
        }
        public bool EditableLegalNameVisible
        {
            get
            {
                return this.divLegalName.Visible;
            }
            set
            {
                if (value)
                {
                    this.divLegalName.Visible = true;
                    this.divDisplayName.Visible = false;

                    if (ContextManager.IsRegistered)
                    {
                        this.ddlLegalPrefix.Enabled = false;
                        this.txtLegalFirstName.Enabled = false;
                        this.txtLegalFirstName.ReadOnly = true;
                        this.txtLegalMiddleName.Enabled = false;
                        this.txtLegalMiddleName.ReadOnly = true;
                        this.txtLegalLastName.Enabled = false;
                        this.txtLegalLastName.ReadOnly = true;
                        this.ddlLegalSuffix.Enabled = false;
                    }
                }
                else
                {
                    this.divLegalName.Visible = false;
                    this.divDisplayName.Visible = true;
                }
            }
        }
        public bool SpecialReqSubSectionVisible
        {
            set { this.lblSpecialReqSubSection.Visible = value; }
        }


        public string PassportIssueAuthorityText
        {
            set { this.lblPassportIssueAuthority.Text = value; }
        }

        public string PassportIssueDateText
        {
            set { this.lblPassportIssueDate.Text = value; }
        }

        public string ProfileCustomFields
        {
            get
            {
                if (!Utilities.IsNothingNullOrEmpty(ViewState["ProfileCF"]))
                {
                    return Convert.ToString(ViewState["ProfileCF"]);
                }
                else
                {
                    return "";
                }
            }
            set { ViewState["ProfileCF"] = value; }
        }

        public string ProfileCustomFieldsRequired
        {
            get
            {
                if (!Utilities.IsNothingNullOrEmpty(ViewState["ProfileCFRequired"]))
                {
                    return Convert.ToString(ViewState["ProfileCFRequired"]);
                }
                else
                {
                    return "";
                }
            }
            set { ViewState["ProfileCFRequired"] = value; }
        }

        public void AddCustomControl()
        {
            ArrayList arrayRequiredItems = new ArrayList();
            bool IsControlVisible = true;
            //'Both CustomFieldsRequired and CustomFields are string data type. The reason is data base schema.
            //'Each string is delimited by (;)
            //'I force to have a exectly 1 item in CustomFieldsRequired for each selected item in CustomFields

            //required items for each CF in PreProfileCustomFields collection
            foreach (string s in ProfileCustomFieldsRequired.Split(new[] { ';' }))
            {
                arrayRequiredItems.Add(s);
            }
            int i = 0;
            //set for array counter

            if (!Utilities.IsNothingNullOrEmpty(ProfileCustomFields))
            {
                System.Web.UI.HtmlControls.HtmlGenericControl mydiv = default(System.Web.UI.HtmlControls.HtmlGenericControl);
                foreach (string cf in ProfileCustomFields.Split(new[] { ';' }))
                {
                    mydiv = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    //Example of how to hide the custom field based on name
                    //If cf = "Division" Or cf = "Region" Then
                    //    IsControlVisible = False
                    //End If
                    AddDynamicCustomControl(mydiv, cf, Convert.ToBoolean(arrayRequiredItems[i]), "cf" + cf, true);

                    mydiv.Attributes.Add("class", "col-xs-12 col-md-6");
                    mydiv.Visible = IsControlVisible;
                    this.pnlCF.Controls.Add(mydiv);

                    i++; // increment counter
                    IsControlVisible = true; // reset
                }
            }
        }
        #endregion

        protected void btnSaveAndContinue_Click(object sender, EventArgs e)
        {
            _Presenter.Save();
        }

        protected void btnExitFinishLater_Click(object sender, EventArgs e)
        {
            Utilities.Navigate(Utilities.NextPage("exit"));
        }
    }
}