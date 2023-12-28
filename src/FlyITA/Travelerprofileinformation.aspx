<%@ Page Title="Fly ITA - Traveler Profile" Language="C#" MasterPageFile="~/_Main.Master"   EnableSessionState="true" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Travelerprofileinformation.aspx.cs" Inherits="FlyITA.Travelerprofileinformation" %>

<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CustomHeaders" runat="Server"></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" runat="Server">


    <h1 class="regTitle">
        <asp:Label ID="lblWelcomeText" runat="server" Text="Traveler Profile <br /><small>Traveler Information</small>"></asp:Label></h1>
    <div id="profile-information" class="row-light">
        <h2>
            <asp:Label ID="lblGeneralInfoSectionHeading" runat="server" Text="Traveler Information Form"></asp:Label></h2>
        <p class="text-danger">
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
        </p>
        <p>The information provided will be used solely to assist us in completing your travel arrangements as quickly and efficiently as possible. It will be appropriately safeguarded and shared only with those individuals making travel arrangements on your behalf.</p>

        <h3 class="subsection">
            <asp:Label ID="lblNameSubSection" runat="server" Text="General Traveler Information"></asp:Label></h3>
        <asp:Panel ID="pnlNameSubSection" runat="server">
            <div id="divName" runat="server" visible="true">
                <div class="row">
                    <div class="col-xs-12 col-md-4">
                        <div class="form-group required">
                            <asp:Label ID="lblTravelerRequired" CssClass="control-label" AssociatedControlID="txtTravelerFirstName" runat="server" Text="Traveler First Name " />
                            <asp:TextBox ID="txtTravelerFirstName" required="required" CssClass="form-control" runat="server" placeholder="Enter Traveler First Name"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-2">
                        <div class="form-group">
                            <asp:Label ID="lblTravelerMiddleName" CssClass="control-label" AssociatedControlID="txtTravelerMiddleName" runat="server" Text="Traveler Middle Name" />
                            <asp:TextBox ID="txtTravelerMiddleName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Traveler Middle Name"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-4">
                        <div class="form-group required">
                            <asp:Label ID="lblTravelerNameRequired" CssClass="control-label" AssociatedControlID="txtTravelerLastName" runat="server" Text="Traveler Last Name" />
                            <asp:TextBox ID="txtTravelerLastName" CssClass="form-control" required="required" runat="server" placeholder="Enter Traveler Last Name"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divDOBGender" runat="server">
                <div class="row">
                    <div class="col-xs-12 col-md-5" id="divDisplayDOB" runat="server">
                        <div id="dvDOBRequired" runat="server" class="form-group required">
                            <asp:Label ID="lblDOBText" AssociatedControlID="txtDateOfBirth" CssClass="control-label" runat="server" Text="Date of Birth"></asp:Label>
                            <div id="divDateOfBirth" class="input-group date" data-date-format="MM/DD/YYYY">
                                <asp:TextBox ID="txtDateOfBirth" required="required" ClientIDMode="Static" CssClass="form-control" MaxLength="10" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                            </div>
                        </div>
                    </div>
                    <div id="divDisplayGender" runat="server" class="col-xs-12 col-md-5">
                        <div id="dvGenderRequired" runat="server" class="form-group required">
                            <asp:Label ID="lblGenderText" runat="server" AssociatedControlID="ddlGender" CssClass="control-label" Text="Gender"></asp:Label>
                            <asp:DropDownList ID="ddlGender" required="required" CssClass="form-control" runat="server">
                                <asp:ListItem Value="">Select an Option</asp:ListItem>
                                <asp:ListItem Value="Male">Male</asp:ListItem>
                                <asp:ListItem Value="FeMale">Female</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divCompanyTitle" runat="server">
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="dvCompanyRequired" runat="server" class="form-group required">
                            <asp:Label ID="lblCompanyText" AssociatedControlID="txtCompanyName" CssClass="control-label" runat="server" Text="Company"></asp:Label>
                            <asp:TextBox ID="txtCompanyName" required="required" CssClass="form-control" runat="server" placeholder="Enter Traveler Company"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="dvTitleRequired" runat="server" class="form-group">
                            <asp:Label ID="lblTitleText" AssociatedControlID="txtTitle" CssClass="control-label" runat="server" Text="Title"></asp:Label>
                            <asp:TextBox ID="txtTitle" CssClass="form-control" runat="server" placeholder="Enter Traveler Job Title"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divDeptCostEmail" runat="server">
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="dvDeptCostCenter" runat="server" class="form-group">
                            <asp:Label ID="lblDeptCostCenterText" runat="server" AssociatedControlID="txtDeptCostCenter" CssClass="control-label" Text="Dept/Cost Center"></asp:Label>
                            <asp:TextBox ID="txtDeptCostCenter" CssClass="form-control" runat="server" placeholder="Enter the Department Cost Center"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-xs-12 col-md-5">
                        <div id="dvReqEmail" runat="server" class="form-group required">
                            <asp:Label ID="lblEmailText" runat="server" AssociatedControlID="txtEmailAddress" CssClass="control-label" Text="Traveler Email"></asp:Label>
                            <asp:TextBox ID="txtEmailAddress" runat="server" MaxLength="128" type="email" placeholder="Enter Traveler Email Address" required="required" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>

            </div>
            <div id="divBusinessPhoneAndFax" runat="server">
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="dvReqBusPhone" runat="server" class="form-group required">
                            <asp:Label ID="lblBusPhoneText" CssClass="control-label" AssociatedControlID="txtBusinessPhone" runat="server" Text="Business Phone"></asp:Label>
                            <asp:TextBox ID="txtBusinessPhone" ClientIDMode="Static" CssClass="form-control telephone" required="required" runat="server" placeholder="Business Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="dvBusFaxRequired" runat="server" class="form-group">
                            <asp:Label ID="lblBusFaxText" AssociatedControlID="txtBusinessFax" CssClass="control-label" runat="server" Text="Business Fax"></asp:Label>
                            <asp:TextBox ID="txtBusinessFax" ClientIDMode="Static" CssClass="form-control telephone" placeholder="Business Fax XXX-XXX-XXXX" runat="server" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divMobilePhoneHomePhone" runat="server">
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="dvMobileRequired" runat="server" class="form-group required">
                            <asp:Label ID="lblBusMobileText" runat="server" AssociatedControlID="txtMobilePhone" CssClass="control-label" Text="Mobile Phone"></asp:Label>
                            <asp:TextBox ID="txtMobilePhone" runat="server" ClientIDMode="Static" CssClass="form-control telephone" data-codeid="#txtMobilePhoneCountryCode" required="required" placeholder="Mobile Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="dvHomeRequired" runat="server" class="form-group">
                            <asp:Label ID="lblHomePhoneText" runat="server" AssociatedControlID="txtHomePhone" CssClass="control-label" Text="Home Phone"></asp:Label>
                            <asp:TextBox ID="txtHomePhone" runat="server" ClientIDMode="Static" CssClass="form-control telephone" data-codeid="#txtHomePhoneCountryCode" placeholder="Home Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

        </asp:Panel>
        <asp:Panel ID="pnlPassport" ClientIDMode="Static" runat="server" CssClass="clear">
            <h3 class="subsection">
                <asp:Label ID="lblPassportSubSection" runat="server" Text="Passport Details"></asp:Label></h3>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <asp:Label ID="lblPassportName" AssociatedControlID="txtPassportName" CssClass="control-label" runat="server" Text="Name as it Appears on Passport"></asp:Label>
                        <asp:TextBox ID="txtPassportName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Name, EXACTLY as it Appears on Passport"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <asp:Label ID="lblPassportNbrText" AssociatedControlID="txtPassportNumber" CssClass="control-label" runat="server" Text="Passport Number"></asp:Label>
                        <asp:TextBox ID="txtPassportNumber" CssClass="form-control" MaxLength="20" runat="server" placeholder="Enter Traveler Passport Number"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <div id="divIssueDate" runat="server">
                            <asp:Label ID="lblPassportIssueDate" AssociatedControlID="txtPassportIssueDate" CssClass="control-label" runat="server" Text="Issue Date"></asp:Label>
                            <div class="input-group" id="passport-issued">
                                <asp:TextBox ID="txtPassportIssueDate" CssClass="form-control" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <div id="divExpirationDate" runat="server">
                            <asp:Label ID="lblPassportExpDateText" AssociatedControlID="txtExpirationDate" CssClass="control-label" runat="server" Text="Expiration Date"></asp:Label>
                            <div class="input-group" id="passport-expiration">
                                <asp:TextBox ID="txtExpirationDate" CssClass="form-control" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <asp:Label ID="lblPlaceofIssue" AssociatedControlID="txtPlaceofIssue" CssClass="control-label" runat="server" Text="Place of Issue"></asp:Label>
                        <asp:TextBox ID="txtPlaceofIssue" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Place of Issue"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlTravelerArrangerInformation" ClientIDMode="Static" runat="server" CssClass="clear">
            <h3 class="subsection">
                <asp:Label ID="lblTravelerArrangerInformation" runat="server" Text="Travel Arranger Information"></asp:Label></h3>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div class="form-group">
                        <asp:Label ID="lblTravelerArrangerName" AssociatedControlID="txtTravelerArrangerName" CssClass="control-label" runat="server" Text="Travel Arranger Name"></asp:Label>
                        <asp:TextBox ID="txtTravelerArrangerName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Travel Arranger Name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="divTravelerArrangerPhone" runat="server" class="form-group">
                        <asp:Label ID="lblTravelerArrangerPhone" runat="server" AssociatedControlID="txtHomePhone" CssClass="control-label" Text="Travel Arranger Phone"></asp:Label>
                        <asp:TextBox ID="txtTravelerArrangerPhone" runat="server" ClientIDMode="Static" CssClass="form-control telephone" data-codeid="#txtTravelerArrangerPhone" placeholder="Travel Arranger Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divTravelerArrangerEmail" runat="server" class="form-group">
                        <asp:Label ID="lblTravelerArrangerEmail" runat="server" AssociatedControlID="txtTravelerArrangerEmail" CssClass="control-label" Text="Travel Arranger Email"></asp:Label>
                        <asp:TextBox ID="txtTravelerArrangerEmail" runat="server" MaxLength="128" type="email" placeholder="Enter Travel Arranger Email" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlERInfo" runat="server">
            <h3 class="subsection">
                <asp:Label ID="lblEmergencySubSecion" runat="server" Text="Emergency Contact Information"></asp:Label></h3>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="dvReqEContactName" runat="server" class="form-group required">
                        <asp:Label ID="lblEContactNameText" CssClass="control-label" AssociatedControlID="txtContactName" runat="server" Text="Contact Name"></asp:Label>
                        <asp:TextBox ID="txtContactName" required="required" CssClass="form-control" runat="server" MaxLength="80" placeholder="Enter Emergency Contact Name"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="dvReqEContactRelation" runat="server" class="form-group required">
                        <asp:Label ID="lblEContactRelationText" CssClass="control-label" AssociatedControlID="ddlContactRelationship" runat="server" Text="Contact Relationship"></asp:Label>
                        <asp:DropDownList ID="ddlContactRelationship" required="required" CssClass="form-control" runat="server">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="Associate" Text="Associate"></asp:ListItem>
                            <asp:ListItem Value="Brother" Text="Brother"></asp:ListItem>
                            <asp:ListItem Value="Child" Text="Child"></asp:ListItem>
                            <asp:ListItem Value="Father" Text="Father"></asp:ListItem>
                            <asp:ListItem Value="Friend" Text="Friend"></asp:ListItem>
                            <asp:ListItem Value="Mother" Text="Mother"></asp:ListItem>
                            <asp:ListItem Value="Other Relative" Text="Other Relative"></asp:ListItem>
                            <asp:ListItem Value="Sister" Text="Sister"></asp:ListItem>
                            <asp:ListItem Value="Spouse" Text="Spouse"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div id="divERDayPhone" runat="server" class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="dvReqEDayPhone" runat="server" class="form-group required">
                        <asp:Label ID="lblEDayPhoneText" runat="server" CssClass="control-label" AssociatedControlID="txtEmergencyContactPhone" Text="Contact Phone Number"></asp:Label>
                        <asp:TextBox ID="txtEmergencyContactPhone" ClientIDMode="Static" CssClass="form-control telephone" required="required" runat="server" placeholder="Emergency Contact Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>


        <asp:Panel ID="pnlCreditCardPurchase" runat="server">
            <h3 class="subsection">
                <asp:Label ID="lblCreditCardPurchaseText" runat="server" Text="Credit Card for Airline Ticket Purchase"></asp:Label></h3>
            <p>A valid credit card will be required before an airline ticket can be confirmed for you. To protect your security, we respectfully request that you call the International Travel Associates office at 800-732-9402 or 515-326-3851 to provide this confidential information. At your request, I.T.A. will update your profile with the information for future reservations. </p>
        </asp:Panel>


        <asp:Panel ID="pnlTravelPreferences" runat="server">
            <h3 class="subsection">
                <asp:Label ID="lblTravelPreferences" runat="server" Text="Travel Preferences"></asp:Label></h3>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divPreferredDepartureAirport" runat="server" class="form-group required">
                        <asp:Label ID="lblPreferredDepartureAirport" CssClass="control-label" AssociatedControlID="ddlPreferredDepartureAirport" runat="server" Text="Preferred Departure Airport"></asp:Label>
                        <asp:DropDownList ID="ddlPreferredDepartureAirport" required="required" CssClass="form-control" runat="server">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Text=" Aberdeen, SD - ABR"></asp:ListItem>
                            <asp:ListItem Text="Abilene, TX - ABI"></asp:ListItem>
                            <asp:ListItem Text="Adak Island, AK - ADK"></asp:ListItem>
                            <asp:ListItem Text="Akron/Canton, OH - CAK"></asp:ListItem>
                            <asp:ListItem Text="Alamosa, CO - ALS"></asp:ListItem>
                            <asp:ListItem Text="Albany, GA - ABY"></asp:ListItem>
                            <asp:ListItem Text="Albany, NY - ALB"></asp:ListItem>
                            <asp:ListItem Text="Albuquerque, NM - ABQ"></asp:ListItem>
                            <asp:ListItem Text="Alexandria, LA - AEX"></asp:ListItem>
                            <asp:ListItem Text="Allentown, PA - ABE"></asp:ListItem>
                            <asp:ListItem Text="Alliance, NE - AIA"></asp:ListItem>
                            <asp:ListItem Text="Alpena, MI - APN"></asp:ListItem>
                            <asp:ListItem Text="Altoona, PA - AOO"></asp:ListItem>
                            <asp:ListItem Text="Amarillo, TX - AMA"></asp:ListItem>
                            <asp:ListItem Text="Anchorage, AK - ANC"></asp:ListItem>
                            <asp:ListItem Text="Appleton, WI - ATW"></asp:ListItem>
                            <asp:ListItem Text="Asheville, NC - AVL"></asp:ListItem>
                            <asp:ListItem Text="Aspen, CO - ASE"></asp:ListItem>
                            <asp:ListItem Text="Athens, GA - AHN"></asp:ListItem>
                            <asp:ListItem Text="Atlanta, GA - ATL"></asp:ListItem>
                            <asp:ListItem Text="Atlantic City, NJ - ACY"></asp:ListItem>
                            <asp:ListItem Text="Augusta, GA - AGS"></asp:ListItem>
                            <asp:ListItem Text="Augusta, ME - AUG"></asp:ListItem>
                            <asp:ListItem Text="Austin, TX - AUS"></asp:ListItem>
                            <asp:ListItem Text="Bakersfield, CA - BFL"></asp:ListItem>
                            <asp:ListItem Text="Baltimore/Washington, MD - BWI"></asp:ListItem>
                            <asp:ListItem Text="Bangor, ME - BGR"></asp:ListItem>
                            <asp:ListItem Text="Bar Harbor, ME - BHB"></asp:ListItem>
                            <asp:ListItem Text="Baton Rouge, LA - BTR"></asp:ListItem>
                            <asp:ListItem Text="Beaumont, TX - BPT"></asp:ListItem>
                            <asp:ListItem Text="Bellingham, WA - BLI"></asp:ListItem>
                            <asp:ListItem Text="Bemidji, MN - BJI"></asp:ListItem>
                            <asp:ListItem Text="Billings, MT - BIL"></asp:ListItem>
                            <asp:ListItem Text="Binghamton, NY - BGM"></asp:ListItem>
                            <asp:ListItem Text="Birmingham, AL - BHM"></asp:ListItem>
                            <asp:ListItem Text="Bismarck, ND - BIS"></asp:ListItem>
                            <asp:ListItem Text="Bloomington, IL - BMI"></asp:ListItem>
                            <asp:ListItem Text="Bluefield, WV - BLF"></asp:ListItem>
                            <asp:ListItem Text="Boise, ID - BOI"></asp:ListItem>
                            <asp:ListItem Text="Boston, MA - BOS"></asp:ListItem>
                            <asp:ListItem Text="Bozeman, MT - BZN"></asp:ListItem>
                            <asp:ListItem Text="Brainerd, MN - BRD"></asp:ListItem>
                            <asp:ListItem Text="Branson, MO - BKG"></asp:ListItem>
                            <asp:ListItem Text="Brookings, SD - BKX"></asp:ListItem>
                            <asp:ListItem Text="Brownsville, TX - BRO"></asp:ListItem>
                            <asp:ListItem Text="Brunswick, GA - BQK"></asp:ListItem>
                            <asp:ListItem Text="Buffalo, NY - BUF"></asp:ListItem>
                            <asp:ListItem Text="Burbank, CA - BUR"></asp:ListItem>
                            <asp:ListItem Text="Burlington, IA - BRL"></asp:ListItem>
                            <asp:ListItem Text="Burlington, VT - BTV"></asp:ListItem>
                            <asp:ListItem Text="Butte, MT - BTM"></asp:ListItem>
                            <asp:ListItem Text="Cape Girardeau, MO - CGI"></asp:ListItem>
                            <asp:ListItem Text="Carlsbad, CA - CLD"></asp:ListItem>
                            <asp:ListItem Text="Carlsbad, NM - CNM"></asp:ListItem>
                            <asp:ListItem Text="Casper, WY - CPR"></asp:ListItem>
                            <asp:ListItem Text="Cedar City, UT - CDC"></asp:ListItem>
                            <asp:ListItem Text="Cedar Rapids, IA - CID"></asp:ListItem>
                            <asp:ListItem Text="Chadron, NE - CDR"></asp:ListItem>
                            <asp:ListItem Text="Champaign, IL - CMI"></asp:ListItem>
                            <asp:ListItem Text="Charleston, SC - CHS"></asp:ListItem>
                            <asp:ListItem Text="Charleston, WV - CRW"></asp:ListItem>
                            <asp:ListItem Text="Charlotte, NC - CLT"></asp:ListItem>
                            <asp:ListItem Text="Charlottesville, VA - CHO"></asp:ListItem>
                            <asp:ListItem Text="Chattanooga, TN - CHA"></asp:ListItem>
                            <asp:ListItem Text="Cheyenne, WY - CYS"></asp:ListItem>
                            <asp:ListItem Text="Chicago/Midway, IL - MDW"></asp:ListItem>
                            <asp:ListItem Text="Chicago/O'Hare, IL - ORD"></asp:ListItem>
                            <asp:ListItem Text="Chico, CA - CIC"></asp:ListItem>
                            <asp:ListItem Text="Cincinnati, OH - CVG"></asp:ListItem>
                            <asp:ListItem Text="Clarksburg, WV - CKB"></asp:ListItem>
                            <asp:ListItem Text="Cleveland, OH - CLE"></asp:ListItem>
                            <asp:ListItem Text="Clovis, NM - CVN"></asp:ListItem>
                            <asp:ListItem Text="Cody, WY - COD"></asp:ListItem>
                            <asp:ListItem Text="College Station, TX - CLL"></asp:ListItem>
                            <asp:ListItem Text="Colorado Springs, CO - COS"></asp:ListItem>
                            <asp:ListItem Text="Columbia, MO - COU"></asp:ListItem>
                            <asp:ListItem Text="Columbia, SC - CAE"></asp:ListItem>
                            <asp:ListItem Text="Columbus, GA - CSG"></asp:ListItem>
                            <asp:ListItem Text="Columbus, MS - GTR"></asp:ListItem>
                            <asp:ListItem Text="Columbus, OH - CMH"></asp:ListItem>
                            <asp:ListItem Text="Corpus Christi, TX - CRP"></asp:ListItem>
                            <asp:ListItem Text="Cortez, CO - CEZ"></asp:ListItem>
                            <asp:ListItem Text="Crescent City, CA - CEC"></asp:ListItem>
                            <asp:ListItem Text="Dallas-Love Field, TX - DAL"></asp:ListItem>
                            <asp:ListItem Text="Dallas/Ft Worth, TX - DFW"></asp:ListItem>
                            <asp:ListItem Text="Dayton, OH - DAY"></asp:ListItem>
                            <asp:ListItem Text="Daytona Beach, FL - DAB"></asp:ListItem>
                            <asp:ListItem Text="Decatur, IL - DEC"></asp:ListItem>
                            <asp:ListItem Text="Del Rio, TX - DRT"></asp:ListItem>
                            <asp:ListItem Text="Denver, CO - DEN"></asp:ListItem>
                            <asp:ListItem Text="Des Moines, IA - DSM"></asp:ListItem>
                            <asp:ListItem Text="Detroit Metro, MI - DTW"></asp:ListItem>
                            <asp:ListItem Text="Devils Lake, ND - DVL"></asp:ListItem>
                            <asp:ListItem Text="Dickinson, ND - DIK"></asp:ListItem>
                            <asp:ListItem Text="Dodge City, KS - DDC"></asp:ListItem>
                            <asp:ListItem Text="Dothan, AL - DHN"></asp:ListItem>
                            <asp:ListItem Text="Du Bois, PA - DUJ"></asp:ListItem>
                            <asp:ListItem Text="Dubuque, IA - DBQ"></asp:ListItem>
                            <asp:ListItem Text="Duluth, MN - DLH"></asp:ListItem>
                            <asp:ListItem Text="Durango, CO - DRO"></asp:ListItem>
                            <asp:ListItem Text="Eau Claire, WI - EAU"></asp:ListItem>
                            <asp:ListItem Text="El Paso, TX - ELP"></asp:ListItem>
                            <asp:ListItem Text="Elko, NV - EKO"></asp:ListItem>
                            <asp:ListItem Text="Elmira, NY - ELM"></asp:ListItem>
                            <asp:ListItem Text="Erie, PA - ERI"></asp:ListItem>
                            <asp:ListItem Text="Escanaba, MI - ESC"></asp:ListItem>
                            <asp:ListItem Text="Eugene, OR - EUG"></asp:ListItem>
                            <asp:ListItem Text="Eureka, CA - ACV"></asp:ListItem>
                            <asp:ListItem Text="Evansville, IN - EVV"></asp:ListItem>
                            <asp:ListItem Text="Fairbanks, AK - FAI"></asp:ListItem>
                            <asp:ListItem Text="Fargo, ND - FAR"></asp:ListItem>
                            <asp:ListItem Text="Farmington, NM - FMN"></asp:ListItem>
                            <asp:ListItem Text="Fayetteville, AR - FYV"></asp:ListItem>
                            <asp:ListItem Text="Fayetteville, NC - FAY"></asp:ListItem>
                            <asp:ListItem Text="Fayetteville/Regional, AR - XNA"></asp:ListItem>
                            <asp:ListItem Text="Flagstaff, AZ - FLG"></asp:ListItem>
                            <asp:ListItem Text="Flint, MI - FNT"></asp:ListItem>
                            <asp:ListItem Text="Florence, SC - FLO"></asp:ListItem>
                            <asp:ListItem Text="Fort Dodge, IA - FOD"></asp:ListItem>
                            <asp:ListItem Text="Fort Myers, FL - RSW"></asp:ListItem>
                            <asp:ListItem Text="Fort Smith, AR - FSM"></asp:ListItem>
                            <asp:ListItem Text="Fort Wayne, IN - FWA"></asp:ListItem>
                            <asp:ListItem Text="Fresno, CA - FAT"></asp:ListItem>
                            <asp:ListItem Text="Ft Lauderdale, FL - FLL"></asp:ListItem>
                            <asp:ListItem Text="Ft. Walton Beach, FL - VPS"></asp:ListItem>
                            <asp:ListItem Text="Gainesville, FL - GNV"></asp:ListItem>
                            <asp:ListItem Text="Garden City, KS - GCK"></asp:ListItem>
                            <asp:ListItem Text="Gillette, WY - GCC"></asp:ListItem>
                            <asp:ListItem Text="Grand Forks, ND - GFK"></asp:ListItem>
                            <asp:ListItem Text="Grand Island, NE - GRI"></asp:ListItem>
                            <asp:ListItem Text="Grand Junction, CO - GJT"></asp:ListItem>
                            <asp:ListItem Text="Grand Rapids, MI - GRR"></asp:ListItem>
                            <asp:ListItem Text="Great Bend, KS - GBD"></asp:ListItem>
                            <asp:ListItem Text="Great Falls, MT - GTF"></asp:ListItem>
                            <asp:ListItem Text="Green Bay, WI - GRB"></asp:ListItem>
                            <asp:ListItem Text="Greensboro/High Point, NC - GSO"></asp:ListItem>
                            <asp:ListItem Text="Greenville, MS - GLH"></asp:ListItem>
                            <asp:ListItem Text="Greenville, NC - PGV"></asp:ListItem>
                            <asp:ListItem Text="Greenville/Spartanburg, SC - GSP"></asp:ListItem>
                            <asp:ListItem Text="Gulfport, MS - GPT"></asp:ListItem>
                            <asp:ListItem Text="Gunnison, CO - GUC"></asp:ListItem>
                            <asp:ListItem Text="Hagerstown, MD - HGR"></asp:ListItem>
                            <asp:ListItem Text="Haines, AK - HNS"></asp:ListItem>
                            <asp:ListItem Text="Hancock Houghton County, MI - CMX"></asp:ListItem>
                            <asp:ListItem Text="Harlingen, TX - HRL"></asp:ListItem>
                            <asp:ListItem Text="Harrisburg, PA - MDT"></asp:ListItem>
                            <asp:ListItem Text="Hartford, CT - BDL"></asp:ListItem>
                            <asp:ListItem Text="Hattiesburg, MS - PIB"></asp:ListItem>
                            <asp:ListItem Text="Hayden, CO - HDN"></asp:ListItem>
                            <asp:ListItem Text="Hays, KS - HYS"></asp:ListItem>
                            <asp:ListItem Text="Helena, MT - HLN"></asp:ListItem>
                            <asp:ListItem Text="Hibbing/Chisholm, MN - HIB"></asp:ListItem>
                            <asp:ListItem Text="Hickory, NC - HKY"></asp:ListItem>
                            <asp:ListItem Text="Hilo, HI - ITO"></asp:ListItem>
                            <asp:ListItem Text="Hilton Head Island, SC - HHH"></asp:ListItem>
                            <asp:ListItem Text="Hobbs, NM - HOB"></asp:ListItem>
                            <asp:ListItem Text="Homer, AK - HOM"></asp:ListItem>
                            <asp:ListItem Text="Honolulu, HI - HNL"></asp:ListItem>
                            <asp:ListItem Text="Hoolehua, Molokai, HI - MKK"></asp:ListItem>
                            <asp:ListItem Text="Hot Springs, AR - HOT"></asp:ListItem>
                            <asp:ListItem Text="Houston Hobby, TX - HOU"></asp:ListItem>
                            <asp:ListItem Text="Houston Intercontinental, TX - IAH"></asp:ListItem>
                            <asp:ListItem Text="Huntington, WV - HTS"></asp:ListItem>
                            <asp:ListItem Text="Huntsville/Decatur, AL - HSV"></asp:ListItem>
                            <asp:ListItem Text="Huron, SD - HON"></asp:ListItem>
                            <asp:ListItem Text="Hyannis, MA - HYA"></asp:ListItem>
                            <asp:ListItem Text="Idaho Falls, ID - IDA"></asp:ListItem>
                            <asp:ListItem Text="Imperial, CA - IPL"></asp:ListItem>
                            <asp:ListItem Text="Indianapolis, IN - IND"></asp:ListItem>
                            <asp:ListItem Text="International Falls, MN - INL"></asp:ListItem>
                            <asp:ListItem Text="Inyokern, CA - IYK"></asp:ListItem>
                            <asp:ListItem Text="Iron Mountain, MI - IMT"></asp:ListItem>
                            <asp:ListItem Text="Ironwood, MI - IWD"></asp:ListItem>
                            <asp:ListItem Text="Islip, NY - ISP"></asp:ListItem>
                            <asp:ListItem Text="Ithaca, NY - ITH"></asp:ListItem>
                            <asp:ListItem Text="Jackson Hole, WY - JAC"></asp:ListItem>
                            <asp:ListItem Text="Jackson, MS - JAN"></asp:ListItem>
                            <asp:ListItem Text="Jackson, TN - MKL"></asp:ListItem>
                            <asp:ListItem Text="Jacksonville, FL - JAX"></asp:ListItem>
                            <asp:ListItem Text="Jacksonville, NC - OAJ"></asp:ListItem>
                            <asp:ListItem Text="Jamestown, ND - JMS"></asp:ListItem>
                            <asp:ListItem Text="Jamestown, NY - JHW"></asp:ListItem>
                            <asp:ListItem Text="Johnstown, PA - JST"></asp:ListItem>
                            <asp:ListItem Text="Joplin, MO - JLN"></asp:ListItem>
                            <asp:ListItem Text="Juneau, AK - JNU"></asp:ListItem>
                            <asp:ListItem Text="Kahului, HI - OGG"></asp:ListItem>
                            <asp:ListItem Text="Kalamazoo, MI - AZO"></asp:ListItem>
                            <asp:ListItem Text="Kalispell, MT - FCA"></asp:ListItem>
                            <asp:ListItem Text="Kansas City, MO - MCI"></asp:ListItem>
                            <asp:ListItem Text="Kapalua, HI - JHM"></asp:ListItem>
                            <asp:ListItem Text="Kearney, NE - EAR"></asp:ListItem>
                            <asp:ListItem Text="Kenai, AK - ENA"></asp:ListItem>
                            <asp:ListItem Text="Ketchikan, AK - KTN"></asp:ListItem>
                            <asp:ListItem Text="Key West, FL - EYW"></asp:ListItem>
                            <asp:ListItem Text="Killeen, TX - GRK"></asp:ListItem>
                            <asp:ListItem Text="Kingman, AZ - IGM"></asp:ListItem>
                            <asp:ListItem Text="Kinston, NC - ISO"></asp:ListItem>
                            <asp:ListItem Text="Klamath Falls, OR - LMT"></asp:ListItem>
                            <asp:ListItem Text="Knoxville, TN - TYS"></asp:ListItem>
                            <asp:ListItem Text="Kodiak, AK - ADQ"></asp:ListItem>
                            <asp:ListItem Text="Kona, HI - KOA"></asp:ListItem>
                            <asp:ListItem Text="La Crosse, WI - LSE"></asp:ListItem>
                            <asp:ListItem Text="Lafayette, LA - LFT"></asp:ListItem>
                            <asp:ListItem Text="Lake Charles, LA - LCH"></asp:ListItem>
                            <asp:ListItem Text="Lake Havasu City, AZ - HII"></asp:ListItem>
                            <asp:ListItem Text="Lake Tahoe, NV - TVL"></asp:ListItem>
                            <asp:ListItem Text="Lanai City, HI - LNY"></asp:ListItem>
                            <asp:ListItem Text="Lancaster, PA - LNS"></asp:ListItem>
                            <asp:ListItem Text="Lansing, MI - LAN"></asp:ListItem>
                            <asp:ListItem Text="Laramie, WY - LAR"></asp:ListItem>
                            <asp:ListItem Text="Laredo, TX - LRD"></asp:ListItem>
                            <asp:ListItem Text="Las Vegas, NV - LAS"></asp:ListItem>
                            <asp:ListItem Text="Latrobe, PA - LBE"></asp:ListItem>
                            <asp:ListItem Text="Lawton, OK - LAW"></asp:ListItem>
                            <asp:ListItem Text="Lewisburg, WV - LWB"></asp:ListItem>
                            <asp:ListItem Text="Lewiston, ID - LWS"></asp:ListItem>
                            <asp:ListItem Text="Lexington, KY - LEX"></asp:ListItem>
                            <asp:ListItem Text="Liberal, KS - LBL"></asp:ListItem>
                            <asp:ListItem Text="Lihue, HI - LIH"></asp:ListItem>
                            <asp:ListItem Text="Lincoln, NE - LNK"></asp:ListItem>
                            <asp:ListItem Text="Little Rock, AR - LIT"></asp:ListItem>
                            <asp:ListItem Text="Long Beach, CA - LGB"></asp:ListItem>
                            <asp:ListItem Text="Longview, TX - GGG"></asp:ListItem>
                            <asp:ListItem Text="Los Angeles, CA - LAX"></asp:ListItem>
                            <asp:ListItem Text="Louisville, KY - SDF"></asp:ListItem>
                            <asp:ListItem Text="Lubbock, TX - LBB"></asp:ListItem>
                            <asp:ListItem Text="Lynchburg, VA - LYH"></asp:ListItem>
                            <asp:ListItem Text="Macon, GA - MCN"></asp:ListItem>
                            <asp:ListItem Text="Madison, WI - MSN"></asp:ListItem>
                            <asp:ListItem Text="Manchester, NH - MHT"></asp:ListItem>
                            <asp:ListItem Text="Manhattan, KS - MHK"></asp:ListItem>
                            <asp:ListItem Text="Manistee, MI - MBL"></asp:ListItem>
                            <asp:ListItem Text="Marion, IL - MWA"></asp:ListItem>
                            <asp:ListItem Text="Marquette, MI - MQT"></asp:ListItem>
                            <asp:ListItem Text="Mason City, IA - MCW"></asp:ListItem>
                            <asp:ListItem Text="McAllen, TX - MFE"></asp:ListItem>
                            <asp:ListItem Text="McCook, NE - MCK"></asp:ListItem>
                            <asp:ListItem Text="Medford, OR - MFR"></asp:ListItem>
                            <asp:ListItem Text="Melbourne, FL - MLB"></asp:ListItem>
                            <asp:ListItem Text="Memphis, TN - MEM"></asp:ListItem>
                            <asp:ListItem Text="Meridian, MS - MEI"></asp:ListItem>
                            <asp:ListItem Text="Miami, FL - MIA"></asp:ListItem>
                            <asp:ListItem Text="Midland/Odessa, TX - MAF"></asp:ListItem>
                            <asp:ListItem Text="Miles City, MT - MLS"></asp:ListItem>
                            <asp:ListItem Text="Milwaukee, WI - MKE"></asp:ListItem>
                            <asp:ListItem Text="Minneapolis/St. Paul, MN - MSP"></asp:ListItem>
                            <asp:ListItem Text="Minot, ND - MOT"></asp:ListItem>
                            <asp:ListItem Text="Missoula, MT - MSO"></asp:ListItem>
                            <asp:ListItem Text="Mobile, AL - MOB"></asp:ListItem>
                            <asp:ListItem Text="Modesto, CA - MOD"></asp:ListItem>
                            <asp:ListItem Text="Moline, IL - MLI"></asp:ListItem>
                            <asp:ListItem Text="Monroe, LA - MLU"></asp:ListItem>
                            <asp:ListItem Text="Monterey/Carmel, CA - MRY"></asp:ListItem>
                            <asp:ListItem Text="Montgomery, AL - MGM"></asp:ListItem>
                            <asp:ListItem Text="Montrose, CO - MTJ"></asp:ListItem>
                            <asp:ListItem Text="Morgantown, WV - MGW"></asp:ListItem>
                            <asp:ListItem Text="Muscle Shoals, AL - MSL"></asp:ListItem>
                            <asp:ListItem Text="Muskegon, MI - MKG"></asp:ListItem>
                            <asp:ListItem Text="Myrtle Beach, SC - MYR"></asp:ListItem>
                            <asp:ListItem Text="Nantucket, MA - ACK"></asp:ListItem>
                            <asp:ListItem Text="Naples, FL - APF"></asp:ListItem>
                            <asp:ListItem Text="Nashville, TN - BNA"></asp:ListItem>
                            <asp:ListItem Text="New Bern, NC - EWN"></asp:ListItem>
                            <asp:ListItem Text="New Haven, CT - HVN"></asp:ListItem>
                            <asp:ListItem Text="New Orleans, LA - MSY"></asp:ListItem>
                            <asp:ListItem Text="New York/ LaGuardia, NY - LGA"></asp:ListItem>
                            <asp:ListItem Text="New York/Kennedy, NY - JFK"></asp:ListItem>
                            <asp:ListItem Text="Newark, NJ - EWR"></asp:ListItem>
                            <asp:ListItem Text="Newburgh, NY - SWF"></asp:ListItem>
                            <asp:ListItem Text="Newport News, VA - PHF"></asp:ListItem>
                            <asp:ListItem Text="Norfolk, VA - ORF"></asp:ListItem>
                            <asp:ListItem Text="North Bend, OR - OTH"></asp:ListItem>
                            <asp:ListItem Text="North Platte, NE - LBF"></asp:ListItem>
                            <asp:ListItem Text="Oakland, CA - OAK"></asp:ListItem>
                            <asp:ListItem Text="Ogdensburg, NY - OGS"></asp:ListItem>
                            <asp:ListItem Text="Oklahoma City, OK - OKC"></asp:ListItem>
                            <asp:ListItem Text="Omaha, NE - OMA"></asp:ListItem>
                            <asp:ListItem Text="Ontario, CA - ONT"></asp:ListItem>
                            <asp:ListItem Text="Orange County, CA - SNA"></asp:ListItem>
                            <asp:ListItem Text="Orlando, FL - MCO"></asp:ListItem>
                            <asp:ListItem Text="Oshkosh, WI - OSH"></asp:ListItem>
                            <asp:ListItem Text="Owensboro, KY - OWB"></asp:ListItem>
                            <asp:ListItem Text="Oxnard/Ventura, CA - OXR"></asp:ListItem>
                            <asp:ListItem Text="Paducah, KY - PAH"></asp:ListItem>
                            <asp:ListItem Text="Palm Springs, CA - PSP"></asp:ListItem>
                            <asp:ListItem Text="Panama City NW Florida Beaches, FL - ECP"></asp:ListItem>
                            <asp:ListItem Text="Panama City, FL - PFN"></asp:ListItem>
                            <asp:ListItem Text="Parkersburg, WV - PKB"></asp:ListItem>
                            <asp:ListItem Text="Pasco, WA - PSC"></asp:ListItem>
                            <asp:ListItem Text="Pellston, MI - PLN"></asp:ListItem>
                            <asp:ListItem Text="Pendleton, OR - PDT"></asp:ListItem>
                            <asp:ListItem Text="Pensacola, FL - PNS"></asp:ListItem>
                            <asp:ListItem Text="Peoria, IL - PIA"></asp:ListItem>
                            <asp:ListItem Text="Philadelphia, PA - PHL"></asp:ListItem>
                            <asp:ListItem Text="Phoenix, AZ - PHX"></asp:ListItem>
                            <asp:ListItem Text="Pierre, SD - PIR"></asp:ListItem>
                            <asp:ListItem Text="Pittsburgh, PA - PIT"></asp:ListItem>
                            <asp:ListItem Text="Pocatello, ID - PIH"></asp:ListItem>
                            <asp:ListItem Text="Port Angeles, WA - CLM"></asp:ListItem>
                            <asp:ListItem Text="Portland, ME - PWM"></asp:ListItem>
                            <asp:ListItem Text="Portland, OR - PDX"></asp:ListItem>
                            <asp:ListItem Text="Prescott, AZ - PRC"></asp:ListItem>
                            <asp:ListItem Text="Presque Isle, ME - PQI"></asp:ListItem>
                            <asp:ListItem Text="Providence, RI - PVD"></asp:ListItem>
                            <asp:ListItem Text="Pueblo, CO - PUB"></asp:ListItem>
                            <asp:ListItem Text="Pullman, WA - PUW"></asp:ListItem>
                            <asp:ListItem Text="Punta Gorda, FL - PGD"></asp:ListItem>
                            <asp:ListItem Text="Quincy, IL - UIN"></asp:ListItem>
                            <asp:ListItem Text="Raleigh/Durham, NC - RDU"></asp:ListItem>
                            <asp:ListItem Text="Rapid City, SD - RAP"></asp:ListItem>
                            <asp:ListItem Text="Reading, PA - RDG"></asp:ListItem>
                            <asp:ListItem Text="Redding, CA - RDD"></asp:ListItem>
                            <asp:ListItem Text="Redmond, OR - RDM"></asp:ListItem>
                            <asp:ListItem Text="Reno, NV - RNO"></asp:ListItem>
                            <asp:ListItem Text="Rhinelander, WI - RHI"></asp:ListItem>
                            <asp:ListItem Text="Richmond, VA - RIC"></asp:ListItem>
                            <asp:ListItem Text="Riverton, WY - RIW"></asp:ListItem>
                            <asp:ListItem Text="Roanoke, VA - ROA"></asp:ListItem>
                            <asp:ListItem Text="Rochester, MN - RST"></asp:ListItem>
                            <asp:ListItem Text="Rochester, NY - ROC"></asp:ListItem>
                            <asp:ListItem Text="Rock Springs, WY - RKS"></asp:ListItem>
                            <asp:ListItem Text="Rockford, IL - RFD"></asp:ListItem>
                            <asp:ListItem Text="Roswell, NM - ROW"></asp:ListItem>
                            <asp:ListItem Text="Sacramento, CA - SMF"></asp:ListItem>
                            <asp:ListItem Text="Saginaw/Bay City, MI - MBS"></asp:ListItem>
                            <asp:ListItem Text="Salem, OR - SLE"></asp:ListItem>
                            <asp:ListItem Text="Salina, KS - SLN"></asp:ListItem>
                            <asp:ListItem Text="Salisbury, MD - SBY"></asp:ListItem>
                            <asp:ListItem Text="Salt Lake City, UT - SLC"></asp:ListItem>
                            <asp:ListItem Text="San Angelo, TX - SJT"></asp:ListItem>
                            <asp:ListItem Text="San Antonio, TX - SAT"></asp:ListItem>
                            <asp:ListItem Text="San Diego, CA - SAN"></asp:ListItem>
                            <asp:ListItem Text="San Francisco, CA - SFO"></asp:ListItem>
                            <asp:ListItem Text="San Jose, CA - SJC"></asp:ListItem>
                            <asp:ListItem Text="San Luis Obispo, CA - SBP"></asp:ListItem>
                            <asp:ListItem Text="Sanford Central FL Reg. Airport - SFB"></asp:ListItem>
                            <asp:ListItem Text="Santa Barbara, CA - SBA"></asp:ListItem>
                            <asp:ListItem Text="Santa Fe, NM - SAF"></asp:ListItem>
                            <asp:ListItem Text="Santa Maria, CA - SMX"></asp:ListItem>
                            <asp:ListItem Text="Santa Rosa, Ca- STS"></asp:ListItem>
                            <asp:ListItem Text="Sarasota, FL - SRQ"></asp:ListItem>
                            <asp:ListItem Text="Sault Ste Marie, MI - CIU"></asp:ListItem>
                            <asp:ListItem Text="Savannah, GA - SAV"></asp:ListItem>
                            <asp:ListItem Text="Scottsbluff, NE - BFF"></asp:ListItem>
                            <asp:ListItem Text="Seattle, WA - SEA"></asp:ListItem>
                            <asp:ListItem Text="Sheridan, WY - SHR"></asp:ListItem>
                            <asp:ListItem Text="Show Low, AZ - SOW"></asp:ListItem>
                            <asp:ListItem Text="Shreveport, LA - SHV"></asp:ListItem>
                            <asp:ListItem Text="Sidney, MT - SDY"></asp:ListItem>
                            <asp:ListItem Text="Sioux City, IA - SUX"></asp:ListItem>
                            <asp:ListItem Text="Sioux Falls, SD - FSD"></asp:ListItem>
                            <asp:ListItem Text="Sitka, AK - SIT"></asp:ListItem>
                            <asp:ListItem Text="South Bend, IN - SBN"></asp:ListItem>
                            <asp:ListItem Text="Spokane, WA - GEG"></asp:ListItem>
                            <asp:ListItem Text="Springfield, IL - SPI"></asp:ListItem>
                            <asp:ListItem Text="Springfield, MO - SGF"></asp:ListItem>
                            <asp:ListItem Text="St Louis, MO - STL"></asp:ListItem>
                            <asp:ListItem Text="St. Cloud, MN - STC"></asp:ListItem>
                            <asp:ListItem Text="St. George, UT - SGU"></asp:ListItem>
                            <asp:ListItem Text="St. Petersburg, FL - PIE"></asp:ListItem>
                            <asp:ListItem Text="State College, PA - SCE"></asp:ListItem>
                            <asp:ListItem Text="Staunton, VA - SHD"></asp:ListItem>
                            <asp:ListItem Text="Steamboat Springs, CO - SBS"></asp:ListItem>
                            <asp:ListItem Text="Stockton, CA - SCK"></asp:ListItem>
                            <asp:ListItem Text="Sun Valley, ID - SUN"></asp:ListItem>
                            <asp:ListItem Text="Syracuse, NY - SYR"></asp:ListItem>
                            <asp:ListItem Text="Tallahassee, FL - TLH"></asp:ListItem>
                            <asp:ListItem Text="Tampa/St. Petersburg, FL - TPA"></asp:ListItem>
                            <asp:ListItem Text="Telluride, CO - TEX"></asp:ListItem>
                            <asp:ListItem Text="Texarkana, AR - TXK"></asp:ListItem>
                            <asp:ListItem Text="Thief River Falls, MN - TVF"></asp:ListItem>
                            <asp:ListItem Text="Toledo, OH - TOL"></asp:ListItem>
                            <asp:ListItem Text="Topeka, KS - FOE"></asp:ListItem>
                            <asp:ListItem Text="Traverse City, MI - TVC"></asp:ListItem>
                            <asp:ListItem Text="Trenton, NJ - TTN"></asp:ListItem>
                            <asp:ListItem Text="Tri Cities, TN - TRI"></asp:ListItem>
                            <asp:ListItem Text="Tucson, AZ - TUS"></asp:ListItem>
                            <asp:ListItem Text="Tulsa, OK - TUL"></asp:ListItem>
                            <asp:ListItem Text="Tupelo, MS - TUP"></asp:ListItem>
                            <asp:ListItem Text="Twin Falls, ID - TWF"></asp:ListItem>
                            <asp:ListItem Text="Tyler, TX - TYR"></asp:ListItem>
                            <asp:ListItem Text="Vail/Eagle, CO - EGE"></asp:ListItem>
                            <asp:ListItem Text="Valdosta, GA - VLD"></asp:ListItem>
                            <asp:ListItem Text="Vernal, UT - VEL"></asp:ListItem>
                            <asp:ListItem Text="Victoria, TX - VCT"></asp:ListItem>
                            <asp:ListItem Text="Waco, TX - ACT"></asp:ListItem>
                            <asp:ListItem Text="Walla Walla, WA - ALW"></asp:ListItem>
                            <asp:ListItem Text="Washington DC Area - WAS"></asp:ListItem>
                            <asp:ListItem Text="Washington Dulles, DC - IAD"></asp:ListItem>
                            <asp:ListItem Text="Washington Reagan Natl, DC - DCA"></asp:ListItem>
                            <asp:ListItem Text="Waterloo, IA - ALO"></asp:ListItem>
                            <asp:ListItem Text="Watertown, NY - ART"></asp:ListItem>
                            <asp:ListItem Text="Watertown, SD - ATY"></asp:ListItem>
                            <asp:ListItem Text="Wausau, WI - CWA"></asp:ListItem>
                            <asp:ListItem Text="Wenatchee, WA - EAT"></asp:ListItem>
                            <asp:ListItem Text="West Palm Beach, FL - PBI"></asp:ListItem>
                            <asp:ListItem Text="Westchester County, NY - HPN"></asp:ListItem>
                            <asp:ListItem Text="Wichita Falls, TX - SPS"></asp:ListItem>
                            <asp:ListItem Text="Wichita, KS - ICT"></asp:ListItem>
                            <asp:ListItem Text="Wilkes Barre/Scranton, PA - AVP"></asp:ListItem>
                            <asp:ListItem Text="Williamsport, PA - IPT"></asp:ListItem>
                            <asp:ListItem Text="Williston, ND - ISN"></asp:ListItem>
                            <asp:ListItem Text="Wilmington, NC - ILM"></asp:ListItem>
                            <asp:ListItem Text="Worland, WY - WRL"></asp:ListItem>
                            <asp:ListItem Text="Yakima, WA - YKM"></asp:ListItem>
                            <asp:ListItem Text="Yankton, SD - YKN"></asp:ListItem>
                            <asp:ListItem Text="Yuma, AZ - YUM	"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="divPreferredCarrier" runat="server" class="form-group">
                        <asp:Label ID="lblPreferredCarrier" CssClass="control-label" AssociatedControlID="ddlPreferredCarrier" runat="server" Text="Preferred Carrier"></asp:Label>
                        <asp:DropDownList ID="ddlPreferredCarrier" CssClass="form-control" runat="server">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="American Airlines" Text="American Airlines"></asp:ListItem>
                            <asp:ListItem Value="Delta Airlines" Text="Delta Airlines"></asp:ListItem>
                            <asp:ListItem Value="Frontier Airlines" Text="Frontier Airlines"></asp:ListItem>
                            <asp:ListItem Value="Jet Blue Airlines" Text="Jet Blue Airlines"></asp:ListItem>
                            <asp:ListItem Value="Southwest Airlines" Text="Southwest Airlines"></asp:ListItem>
                            <asp:ListItem Value="United Airlines" Text="United Airlines"></asp:ListItem>
                            <asp:ListItem Value="Other" Text="Other"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-xs-12">
                        <div id="divOtherPreferredCarrier" runat="server" class="form-group required" visible="False">
                            <asp:Label ID="Label3" CssClass="control-label" AssociatedControlID="txtOtherPreferredCarrier" runat="server" Text="Other Preferred Carrier"></asp:Label>
                            <asp:TextBox ID="txtOtherPreferredCarrier" CssClass="form-control" runat="server" MaxLength="80" placeholder="Enter your Other Preferred Carrier"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divSeatingPreference" runat="server" class="form-group required">
                        <asp:Label ID="lblSeatingPreference" CssClass="control-label" AssociatedControlID="ddlPreferredCarrier" runat="server" Text="Seating Preference"></asp:Label>
                        <asp:DropDownList ID="ddlSeatingPreference" CssClass="form-control" runat="server" required="required">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="Aisle" Text="Aisle"></asp:ListItem>
                            <asp:ListItem Value="Window" Text="Window"></asp:ListItem>
                            <asp:ListItem Value="No Preference" Text="No Preference"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="divSpecialMealRequirements" runat="server" class="form-group">
                        <asp:Label ID="lblSpecialMealRequirements" CssClass="control-label" AssociatedControlID="txtSpecialMealRequirements" runat="server" Text="Special Meal Requirements"></asp:Label>
                        <asp:TextBox ID="txtSpecialMealRequirements" CssClass="form-control" runat="server" placeholder="Enter Any Special Meal Requirements"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-12">
                    <asp:Repeater ID="rptPerson" runat="server" OnItemDataBound="rptPerson_ItemDataBound">
                        <ItemTemplate>
                            <div class="person">
                                <div class="frequent-flier">
                                    <asp:Repeater ID="rptFrequentFlyerNumber" runat="server" OnItemDataBound="rptFrequentFlyerNumber_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="row frequent-flier-number">

                                                <div class="form-group clearfix">
                                                    <div class="col-xs-12">
                                                        <asp:Label ID="lblFFText" runat="server" CssClass="control-label" AssociatedControlID="txtFrequentFlyerNumber" Text="Frequent Flyer Number"></asp:Label>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5 append-bottom-xs append-bottom-sm">
                                                        <asp:DropDownList ID="ddlFrequentFlyerNumber" CssClass="form-control" runat="server"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5">
                                                        <asp:TextBox ID="txtFrequentFlyerNumber" CssClass="form-control" runat="server" placeholder="Enter Frequent Flyer Number"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div class="row">
                                        <div class="form-group clearfix">
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm remove-flier btn-block"><span class="glyphicon glyphicon-minus"></span>Remove</button>
                                            </div>
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm add-flier btn-block"><span class="glyphicon glyphicon-plus"></span>Add</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel1" runat="server">
            <h3 class="subsection">
                <asp:Label ID="Label4" runat="server" Text="Hotel Preferences"></asp:Label></h3>
            <p>A valid credit card will be required before a hotel reservation can be confirmed for you. To protect your security, we respectfully request that you call the International Travel Associates office at 800-732-9402 or 515-326-3851 to provide this confidential information. At your request, I.T.A. will update your profile with the information for future reservations.</p>

            <div class="row">
                <div class="col-xs-12 col-md-12">
                    <asp:Repeater ID="rptPersonHotelClubMemberships" runat="server" OnItemDataBound="rptPersonHotelClubMemberships_ItemDataBound">
                        <ItemTemplate>
                            <div class="person">

                                <div class="frequent-flier">
                                    <asp:Repeater ID="rptHotelClubMembershipsNumber" runat="server" OnItemDataBound="rptHotelClubMembershipsNumber_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="row frequent-flier-number">

                                                <div class="form-group clearfix">
                                                    <div class="col-xs-12">
                                                        <asp:Label ID="lblFFText" runat="server" CssClass="control-label" AssociatedControlID="ddlHotelClubMemberships" Text="Hotel Club Memberships"></asp:Label>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5 append-bottom-xs append-bottom-sm">
                                                        <asp:DropDownList ID="ddlHotelClubMemberships" CssClass="form-control" runat="server"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5">
                                                        <asp:TextBox ID="txtHotelClubMemberships" CssClass="form-control" runat="server" placeholder="Enter Hotel Club Memberships"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div class="row">
                                        <div class="form-group clearfix">
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm remove-flier btn-block"><span class="glyphicon glyphicon-minus"></span>Remove</button>
                                            </div>
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm add-flier btn-block"><span class="glyphicon glyphicon-plus"></span>Add</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divOtherHotelMemberShip" runat="server" class="form-group">
                        <asp:Label ID="lblOtherHotelMemberShip" runat="server" AssociatedControlID="txtOtherHotelMemberShip" CssClass="control-label" Text="Other Hotel Club Membership"></asp:Label>
                        <asp:TextBox ID="txtOtherHotelMemberShip" runat="server" placeholder="Enter Other Hotel Club Membership" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="divOtherHotelClubMembershipsNumber" runat="server" class="form-group">
                        <asp:Label ID="lblOtherHotelClubMembershipsNumber" runat="server" AssociatedControlID="txtOtherHotelClubMembershipsNumber" CssClass="control-label" Text="Other Hotel Club Membership Number"></asp:Label>
                        <asp:TextBox ID="txtOtherHotelClubMembershipsNumber" runat="server" placeholder="Enter Other Hotel Club Membership Number" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divSmokingPreference" runat="server" class="form-group required">
                        <asp:Label ID="Label5" CssClass="control-label" AssociatedControlID="ddlSmokingPreference" runat="server" Text="Smoking Preference"></asp:Label>
                        <asp:DropDownList ID="ddlSmokingPreference" CssClass="form-control" runat="server" required="required">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="Non-Smoking" Text="Non-Smoking"></asp:ListItem>
                            <asp:ListItem Value="Smoking" Text="Smoking"></asp:ListItem>
                            <asp:ListItem Value="No Preference" Text="No Preference"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-md-5">
                    <div id="divBedPreference" runat="server" class="form-group required">
                        <asp:Label ID="lblBedPreference" CssClass="control-label" AssociatedControlID="ddlSmokingPreference" runat="server" Text="Bed Preference"></asp:Label>
                        <asp:DropDownList ID="ddlBedPreference" CssClass="form-control" runat="server" required="required">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="King" Text="King"></asp:ListItem>
                            <asp:ListItem Value="Double/Double" Text="Double/Double"></asp:ListItem>
                            <asp:ListItem Value="No Preference" Text="No Preference"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divSpecialRequirements" runat="server" class="form-group">
                        <asp:Label ID="lblSpecialRequirements" runat="server" AssociatedControlID="txtSpecialRequirements" CssClass="control-label" Text="Special Requirements"></asp:Label>
                        <asp:TextBox ID="txtSpecialRequirements" runat="server" placeholder="Enter Any Special Requirements" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
            <h3 class="subsection">
                <asp:Label ID="Label6" runat="server" Text="Rental Car Preferences "></asp:Label></h3>
            <div class="row">
                <div class="col-xs-12 col-md-5">
                    <div id="divVehicleSize" runat="server" class="form-group">
                        <asp:Label ID="Label7" CssClass="control-label" AssociatedControlID="ddlVehicleSize" runat="server" Text="Vehicle Size"></asp:Label>
                        <asp:DropDownList ID="ddlVehicleSize" CssClass="form-control" runat="server">
                            <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                            <asp:ListItem Value="Luxury" Text="Luxury"></asp:ListItem>
                            <asp:ListItem Value="Full-size" Text="Full-size"></asp:ListItem>
                            <asp:ListItem Value="Mid-size" Text="Mid-size"></asp:ListItem>
                            <asp:ListItem Value="Economy" Text="Economy"></asp:ListItem>
                            <asp:ListItem Value="Specialty" Text="Specialty (i.e. SUV, Minivan)"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-12 col-md-12">
                    <asp:Repeater ID="rptRentalCar" runat="server" OnItemDataBound="rptRentalCar_ItemDataBound">
                        <ItemTemplate>
                            <div class="person">

                                <div class="frequent-flier">
                                    <asp:Repeater ID="rptRentalCarMemberships" runat="server" OnItemDataBound="rptRentalCarMemberships_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="row frequent-flier-number">

                                                <div class="form-group clearfix">
                                                    <div class="col-xs-12">

                                                        <asp:Label ID="lblFFText" runat="server" CssClass="control-label" AssociatedControlID="txtRentalCarMemberships" Text="Rental Car Membership"></asp:Label>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5 append-bottom-xs append-bottom-sm">
                                                        <asp:DropDownList ID="ddlRentalCarMemberships" CssClass="form-control" runat="server"></asp:DropDownList>
                                                    </div>
                                                    <div class="col-xs-12 col-md-5">
                                                        <asp:TextBox ID="txtRentalCarMemberships" CssClass="form-control" runat="server" placeholder="Enter Rental Car Membership Number"></asp:TextBox>
                                                    </div>

                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div class="row">
                                        <div class="form-group clearfix">
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm remove-flier btn-block"><span class="glyphicon glyphicon-minus"></span>Remove</button>
                                            </div>
                                            <div class="col-xs-6 col-md-2">
                                                <button class="btn btn-info btn-sm add-flier btn-block"><span class="glyphicon glyphicon-plus"></span>Add</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel3" runat="server">
            <h3 class="subsection">
                <asp:Label ID="Label1" runat="server" Text="CAPTCHA"></asp:Label></h3>

            <div class="col-xs-12 col-md-12">

                <div>
                    <strong>To submit, type the characters you see in the picture below</strong>
                    <cc1:CaptchaControl ID="cptCaptcha" runat="server" CaptchaBackgroundNoise="Low" CaptchaLength="5" CaptchaHeight="60" CaptchaWidth="200" CaptchaLineNoise="None" CaptchaMinTimeout="5" CaptchaMaxTimeout="240" FontColor="#529E00"
                                        ErrorInputTooFast="Image text was typed too quickly. " ErrorInputTooSlow="Image text was typed too slowly." EnableViewState="False" />
                </div>
                <br />
                <div class="col-xs-12 col-md-5 required">
                    <div id="div1" runat="server" class="form-group required">
                        <asp:Label CssClass="control-label" AssociatedControlID="txtCaptcha" runat="server" Text="Characters"> </asp:Label>
                        <asp:TextBox ID="txtCaptcha" CssClass="form-control" runat="server" required="required"></asp:TextBox>
                    </div>
                </div>
                <br />

            </div>
            <div class="form-group prepend-top">

                <asp:Button ID="btnSaveAndContinue" runat="server" Text="Submit" CssClass="btn btn-success submit-button" data-loading-text="Please Wait..." OnClick="btnSubmit_Click"></asp:Button>
            </div>

            <div>&nbsp;</div>
            </br>
            </br>
        </asp:Panel>
    </div>
    <script src="scripts/guestinformation.js"></script>
    <script src="scripts/formvalidation.js"></script>
</asp:Content>
