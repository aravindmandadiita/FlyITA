<%@ Page Title="Profile - Guest Information (continued)" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="GuestProfileInformation.aspx.cs" Inherits="FlyITA.GuestProfileInformation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomHeaders" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <h1 class="regTitle">
        <asp:Label ID="lblWelcomeText" runat="server" Text="Registration <br /><small>Guest Information (continued)</small>"></asp:Label></h1>
    <p class="text-danger">
        <asp:Label ID="lblInfo" CssClass="warning" runat="server"></asp:Label>
    </p>
    <h3>
        <asp:Label ID="lblGeneralInfoSectionHeading" runat="server" Text="General Information"></asp:Label></h3>
    <h3 class="subsection">
        <asp:Label ID="lblNameSubSection" runat="server" Text="Guest Name:"></asp:Label></h3>
    <div id="divLegalName" runat="server" visible="false">
        <div id="paragraphLegalNameVerbiage" runat="server" class="alert alert-warning">
            <strong>Important!</strong>
            <asp:Label ID="lblLegalNameVerbiage" runat="server" Text="Legal Name must match the passport or government issued photo ID your guest will be using for travel."></asp:Label>
        </div>
        <div class="row">
            <div id="divddlPrefix" runat="server" class="col-xs-12 col-md-4">
                <div class="form-group">
                    <asp:Label ID="lblPrefix" AssociatedControlID="ddlLegalPrefix" CssClass="control-label" runat="server" Text="Prefix"></asp:Label>
                    <asp:DropDownList ID="ddlLegalPrefix" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div id="divNoPrefix" runat="server">&nbsp;</div>
        </div>
        <div class="row">
            <div class="col-xs-12 col-md-3">
                <div class="form-group required">
                    <asp:Label ID="lblFirstNameRequired" CssClass="control-label" AssociatedControlID="txtLegalFirstName" runat="server" Text="First Name" />
                    <asp:TextBox ID="txtLegalFirstName" CssClass="form-control" required="required" MaxLength="40" runat="server" placeholder="Enter Your Guest's First Name"></asp:TextBox>
                </div>
            </div>
            <div class="col-xs-12 col-md-3">
                <div class="form-group">
                    <asp:Label ID="dvLegalMiddleName" CssClass="control-label" AssociatedControlID="txtLegalMiddleName" runat="server" Text="Middle" />
                    <asp:TextBox ID="txtLegalMiddleName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter your Guest's Middle Name"></asp:TextBox>
                </div>
            </div>
            <div class="col-xs-12 col-md-3">
                <div class="form-group required">
                    <asp:Label ID="lblLastNameRequired" CssClass="control-label" AssociatedControlID="txtLegalLastName" runat="server" Text="Last Name" />
                    <asp:TextBox ID="txtLegalLastName" CssClass="form-control" required="required" MaxLength="40" runat="server"></asp:TextBox>
                </div>
            </div>
            <div id="divddlSufix" runat="server" class="col-xs-12 col-md-3">
                <div class="form-group">
                    <asp:Label ID="lblSufix" runat="server" CssClass="control-label" AssociatedControlID="ddlLegalSuffix" Text="Suffix"></asp:Label>
                    <asp:DropDownList ID="ddlLegalSuffix" CssClass="form-control" runat="server"></asp:DropDownList>
                </div>
            </div>
        </div>
    </div>
    <div id="divDisplayName" runat="server" visible="true" class="append-bottom">
        <h4>
            <asp:Label ID="lblDisplayName" runat="server" Text="Legal Name:"></asp:Label></h4>
        <asp:Label ID="lblName" runat="server"></asp:Label>
        <div>
            <a href="#" id="legal-name">
                <asp:Label ID="lblLegalName" runat="server" Text="Not Your Guest's Legal Name?"></asp:Label></a>
        </div>
    </div>
    <div class="row" id="divNickName" runat="server">
        <div class="col-xs-12 col-md-4">
            <div class="form-group">
                <asp:Label ID="lblNicknameText" runat="Server" AssociatedControlID="txtNickname" CssClass="control-label" Text="Nickname:"></asp:Label>
                <asp:TextBox ID="txtNickname" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter your Guest's Nickname"></asp:TextBox>
            </div>
        </div>
    </div>
    <div id="divBadgeName" runat="server">
        <h4>Name Badge:</h4>
        <div class="row">
            <div class="col-xs-12 col-md-4">
                <div id="dvBadgeFirstRequired" runat="server" class="form-group required">
                    <asp:Label ID="lblBadgeFNameText" CssClass="control-label" AssociatedControlID="txtBadgeFirstName" runat="server" Text="First Name"></asp:Label>
                    <asp:TextBox ID="txtBadgeFirstName" CssClass="form-control" required="required" MaxLength="40" runat="server" placeholder="Enter Name Badge First Name"></asp:TextBox>
                </div>
            </div>
            <div class="col-xs-12 col-md-4">
                <div id="dvBadgeLastRequired" runat="server" class="form-group required">
                    <asp:Label ID="lblBadgeLNameText" CssClass="control-label" AssociatedControlID="txtBadgeLastName" runat="server" Text="Last Name"></asp:Label>
                    <asp:TextBox ID="txtBadgeLastName" CssClass="form-control" required="required" MaxLength="40" runat="server" placeholder="Enter Name Badge Last Name"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <asp:Panel ID="pnlDisplayPersonalInfoHeader" runat="server">
        <h3 class="subsection">
            <asp:Label ID="lblPersonalInfoSubSectionText" runat="server" Text="Personal Information:"></asp:Label></h3>
        <div id="divDisplayDOB" runat="server" class="row">
            <div class="col-xs-12 col-md-4">
                <div id="dvDOBRequired" runat="server" class="form-group required">
                    <asp:Label ID="lblDOBText" CssClass="control-label" AssociatedControlID="txtDateOfBirth" runat="server" Text="Date of Birth"></asp:Label>
                    <div class="input-group date" id="divDateOfBirth" data-date-format="MM/DD/YYYY">
                        <asp:TextBox ID="txtDateOfBirth" CssClass="form-control" required="required" runat="server" placeholder="Enter Guest Date of Birth"></asp:TextBox>
                        <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                    </div>
                </div>
            </div>
        </div>
        <div id="divDisplayGender" runat="server" class="row">
            <div class="col-xs-12 col-md-4">
                <div id="dvGenderRequired" runat="server" class="form-group required">
                    <asp:Label ID="lblGenderText" AssociatedControlID="ddlGender" CssClass="control-label" runat="server" Text="Gender"></asp:Label>
                    <asp:DropDownList ID="ddlGender" CssClass="form-control" required="required" runat="server"></asp:DropDownList>
                </div>
            </div>
        </div>
    </asp:Panel>
    <!--used to add dinamic CF controls -->
    <asp:Panel ID="pnlCF" runat="server" class="row"></asp:Panel>
    <asp:Panel ID="pnlProofOfCitizenship" runat="server">
        <h2>
            <asp:Label ID="lblPOCSectionHeader" runat="server" Text="Proof of Citizenship"></asp:Label></h2>
        <h3 class="subsection">
            <asp:Label ID="lblPOCSubSection" runat="server" Text="General:"></asp:Label></h3>
        <div class="row">
            <div class="col-xs-12 col-md-4">
                <div id="dvProofOfCitizenshipReq" runat="server" class="form-group required">
                    <asp:Label ID="lblPOCTypeText" CssClass="control-label" AssociatedControlID="ddlProofofCitizenshipType" runat="server" Text="Proof of Citizenship Type" />
                    <asp:Label ID="lblProofOfCitizenshipType" CssClass="clearfix" runat="server" Visible="false" />
                    <asp:DropDownList ID="ddlProofofCitizenshipType" ClientIDMode="Static" CssClass="form-control" required="required" runat="server"></asp:DropDownList>
                </div>
            </div>
        </div>
        <p>Government Issued</p>
    </asp:Panel>
    <asp:Panel ID="pnlPassport" ClientIDMode="Static" runat="server" CssClass="hidden">
        <asp:HiddenField runat="server" ClientIDMode="Static" ID="pnlPassportVisibility" Value="false" />
        <h3 class="subsection">
            <asp:Label ID="lblPassportSubSection" runat="server" Text="Passport Details:"></asp:Label></h3>
        <div class="row">
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Label ID="lblPassportAvailableText" CssClass="control-label" AssociatedControlID="rbPassportStatusYes" runat="server" Text="Do you have your passport information available now?"></asp:Label>
                    <span class="clearfix"></span>
                    <div class="btn-group" data-toggle="buttons">
                        <label class="btn btn-primary">
                            <asp:RadioButton ID="rbPassportStatusYes" ClientIDMode="Static" GroupName="rbgPassportInformation" runat="server" />
                            <asp:Label runat="server" ID="lblPassportStatusYes" Text="Yes" />
                        </label>
                        <label class="btn btn-primary">
                            <asp:RadioButton ID="rbPassportStatusNo" ClientIDMode="Static" GroupName="rbgPassportInformation" runat="server" />
                            <asp:Label runat="server" ID="lblPassportStatusNo" Text="No, I will provide it later." />
                        </label>
                    </div>
                </div>
            </div>
        </div>
        <div id="passport-info">
            <div class="alert alert-warning"><strong>Important!</strong> Passport Name must match Legal Name above.</div>
            <h4>Passport Name:</h4>
            <div class="row">
                <div class="col-xs-12 col-md-3">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportFNameText" AssociatedControlID="txtFirstPassportName" CssClass="control-label" runat="server" Text="First Name"></asp:Label>
                        <asp:TextBox ID="txtFirstPassportName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Passport First Name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-3">
                    <div class="form-group">
                        <asp:Label ID="lblPassportMNameText" AssociatedControlID="txtMiddlePassportName" CssClass="control-label" runat="server" Text="Middle"></asp:Label>
                        <asp:TextBox ID="txtMiddlePassportName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Passport Middle Name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-3">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportLNameText" AssociatedControlID="txtLastPassportName" CssClass="control-label" runat="server" Text="Last Name"></asp:Label>
                        <asp:TextBox ID="txtLastPassportName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Passport Last Name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-3">
                    <div class="form-group">
                        <asp:Label ID="lblPassportSuffixText" AssociatedControlID="ddlSuffixPassport" CssClass="control-label" runat="server" Text="Suffix"></asp:Label>
                        <asp:DropDownList ID="ddlSuffixPassport" CssClass="form-control" runat="server"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-4">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportNbrText" runat="server" AssociatedControlID="txtPassportNumber" CssClass="control-label" Text="Passport Number"></asp:Label>
                        <asp:TextBox ID="txtPassportNumber" CssClass="form-control" MaxLength="20" runat="server" placeholder="Enter Passport Number"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-6">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportExpDateText" AssociatedControlID="txtExpirationDate" CssClass="control-label" runat="server" Text="Expiration Date"></asp:Label><br />
                        <div class="input-group date" id="passport-expiration" data-date-format="MM/DD/YYYY">
                            <asp:TextBox ID="txtExpirationDate" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder="Enter a Passport Expiration Date"></asp:TextBox>
                            <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                        </div>
                    </div>
                </div>
                <div id="divIssueDate" runat="server" class="col-xs-12 col-md-6">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportIssueDate" AssociatedControlID="txtPassportIssueDate" CssClass="control-label" runat="server" Text="Issue Date"></asp:Label><br />
                        <div class="input-group date" id="passport-issued" data-date-format="MM/DD/YYYY">
                            <asp:TextBox ID="txtPassportIssueDate" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder="Enter a Passport Issue Date"></asp:TextBox>
                            <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-4">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportIssuedText" CssClass="control-label" AssociatedControlID="ddlCountryOfIssue" runat="server" Text="Country of Issue"></asp:Label>
                        <asp:DropDownList ID="ddlCountryOfIssue" CssClass="form-control" runat="server"></asp:DropDownList>
                    </div>
                </div>
            </div>
            <div id="divPassportIssueAuth" runat="server" class="row">
                <div class="col-xs-12 col-md-4">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportIssueAuthority" AssociatedControlID="txtPassportIssueAuthority" CssClass="control-label" runat="server" Text="Issuing Authority"></asp:Label><br />
                        <asp:TextBox ID="txtPassportIssueAuthority" CssClass="form-control" MaxLength="20" runat="server" placeholder="Enter a Passport Issuing Authority"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-4">
                    <div class="form-group required">
                        <asp:Label ID="lblPassportNationText" AssociatedControlID="ddlNationality" CssClass="control-label" runat="server" Text="Nationality"></asp:Label>
                        <asp:DropDownList ID="ddlNationality" CssClass="form-control" runat="server"></asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlBirthCertificate" runat="server" Visible="false">
        <h3 class="formheading2">
            <asp:Label ID="lblBirthCSubSection" runat="server" Text="Birth Certificate Details"></asp:Label></h3>

        &nbsp;<asp:Label ID="lblBCertNationText" runat="server" Text="Birth Certificate Nationality"></asp:Label><br />
        <asp:DropDownList ID="ddlBirthCertificate" runat="server"></asp:DropDownList>

    </asp:Panel>
    <asp:Panel ID="pnlSpecialRequest" runat="server">
        <h3 class="subsection">
            <asp:Label ID="lblSpecialReqSubSection" runat="server" Text="Special Requests"></asp:Label></h3>

        <asp:UpdatePanel ID="upanelDiet" runat="server">
            <ContentTemplate>

                <div class="col-xs-12 col-md-4 ">
                    <div class="form-inline">

                        <asp:Label ID="lblDietPrefText" Font-Bold="true" runat="server" CssClass="control-label" Bold="true" Text="<b> Dietary Restrictions?: </b>"></asp:Label>

                        <asp:CheckBoxList ID="cblSpecialDiet" CssClass="radio-list" AutoPostBack="True" runat="server">
                              <asp:ListItem Value="Celiac/Gluten Allergy"></asp:ListItem>
                                <asp:ListItem Value="Dairy/Lactose Allergy"></asp:ListItem>
                                <asp:ListItem Value="Egg Allergy"></asp:ListItem>
                                <asp:ListItem Value="Kosher"></asp:ListItem>
                                <asp:ListItem Value="Nut Allergy"></asp:ListItem>
                                <asp:ListItem Value="Shellfish Allergy"></asp:ListItem>
                                <asp:ListItem Value="Vegan"></asp:ListItem>
                                <asp:ListItem Value="Vegetarian"></asp:ListItem>
                                <asp:ListItem Value="Other"></asp:ListItem>
                                <asp:ListItem Value="None"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                    <div>
                    </div>
                    <asp:Panel runat="server" ID="pnlOtherDiet" Visible="false" CssClass="clear form-group">
                        <asp:Label ID="lblOther" AssociatedControlID="txtOtherDietaryPreferences" CssClass="control-label" runat="server" Text="<span class='glyphicon glyphicon-asterisk required-demo'></span> Other Dietary Requirements"></asp:Label>
                        <asp:TextBox ID="txtOtherDietaryPreferences" CssClass="form-control" Rows="3" runat="server" TextMode="MultiLine" placeholder="Enter your Other Dietary Requirements"></asp:TextBox>


                    </asp:Panel>
                </div>

                <div id="divPhysPrefVisible" runat="server" class="row">
                    <div class="col-xs-12 col-md-4">
                        <div class="form-group">
                            <asp:Label ID="lblPhysicalReqText" AssociatedControlID="chkSpecificAids" CssClass="control-label" runat="server" Text="Specific Aids or Services Required?:"></asp:Label>
                            <asp:CheckBoxList ID="chkSpecificAids" CssClass="radio-list" AutoPostBack="True" runat="server">
                                <asp:ListItem Value="Visual Aid/Services"></asp:ListItem>
                                <asp:ListItem Value="Audio Aid/Services"></asp:ListItem>
                                <asp:ListItem Value="Mobility Aid/Services"></asp:ListItem>
                                <asp:ListItem Value="Other"></asp:ListItem>
                                <asp:ListItem Value="None"></asp:ListItem>
                            </asp:CheckBoxList>

                        </div>
                        <div>
                        </div>
                        <asp:Panel runat="server" ID="pnlOtherSpecificAids" Visible="false" CssClass="clear form-group">
                            <asp:Label ID="lblOtherSpecificAids" AssociatedControlID="txtOtherSpecificAids" CssClass="control-label" runat="server" Text="<span class='glyphicon glyphicon-asterisk required-demo'></span> Other Specific Aids or Services"></asp:Label>
                            <asp:TextBox ID="txtOtherSpecificAids" CssClass="form-control" Rows="3" runat="server" TextMode="MultiLine" placeholder="Enter your Other Specific Aids or Services"></asp:TextBox>

                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cblSpecialDiet" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="chkSpecificAids" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>

    </asp:Panel>
    <div class="form-group prepend-top">
        <asp:Button ID="btnSaveAndContinue" runat="server" Text="Save and Continue" CssClass="btn btn-success submit-button" data-loading-text="Please Wait..." OnClick="btnSaveAndContinue_Click"></asp:Button>
        <asp:Button ID="btnExitFinishLater" UseSubmitBehavior="false" runat="server" Text="Exit and Finish Later" CssClass="btn btn-danger cancel-button" OnClick="btnExitFinishLater_Click"></asp:Button>
    </div>
    <div class="modal fade" id="legalNameModal" tabindex="-1" role="dialog" aria-labelledby="legalNameModalLabel" aria-hidden="true" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" data-dismiss="modal" class="close">&times;</button>
                    <h4 class="modal-title"></h4>
                </div>
                <div class="modal-body"></div>
                <div class="modal-footer">
                    <button class="btn btn-primary" data-dismiss="modal">
                        <span class="glyphicon glyphicon-check">OK</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
    <script src="scripts/formvalidation.js"></script>
    <script src="scripts/profileinformation.js"></script>
</asp:Content>
