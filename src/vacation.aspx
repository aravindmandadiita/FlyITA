<%@ Page Title="Vacation Travel" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="vacation.aspx.cs" Inherits="FlyITA.vacation" %>
<%@ Register TagPrefix="cc1" Namespace="MSCaptcha" Assembly="MSCaptcha, Version=2.0.1.36094, Culture=neutral, PublicKeyToken=b9ff12f28cdcf412" %>
<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomHeaders" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <h1>Vacation Travel<br />
        <small>When it's time to relax</small></h1>
    <div class="row-light">
        <div class="container">
            <p class="intro">We  know you don&rsquo;t always travel for business…Sometimes, you get to relax and do  the things you enjoy most. Whether you like to play in the snow or the sand or  be on slopes, the green or the sea, we can help.</p>
        </div>
    </div>
    <div class="row-dark">
        <div class="container">
            <div class="col-xs-12 col-sm-6">
                <h3>We&rsquo;ll Do  the Heavy Lifting</h3>
                <p>
                    Why spend hours online trying to figure out how  to plan the perfect vacation? ITA has leisure travel
                specialists who can do the  research for you. We&rsquo;ll put together trip options and then meet
                with you to  review your choices. Just call our Leisure Travel Department directly at (800) 622-4099,
                email a request to <a href="mailto:leisuretravel@itagroup.com">leisuretravel@itagroup.com</a>, or
                simply complete  the online form below, and someone will contact you.
                </p>
            </div>
            <div class="col-xs-12 col-sm-6">
                <h3>Give Them the World!</h3>
                <p>
                    What better  gift can you give someone than the entire world? We have gift certificates that  make
                excellent retirement, graduation or wedding gifts. Come to think of it,  whatever the occasion,
                who wouldn&rsquo;t want a gift that gives them unlimited travel  possibilities? Let their imagination run wild! Call (800) 622-4099 or email <a href="mailto:leisuretravel@itagroup.com">leisuretravel@itagroup.com</a> for more information about our gift certificate program.
                </p>
            </div>
        </div>
    </div>
    <div class="row-light">
        <div class="container">
            <h2>
                <asp:Label ID="lblGeneralInfoSectionHeading" runat="server" Text="VACATION TRAVEL REQUEST FORM"></asp:Label></h2>
            <p class="text-danger">
                <asp:Label ID="lblInfo" runat="server"></asp:Label>
            </p>
            <p>The information provided will be used solely to assist us in completing your travel arrangements as quickly and efficiently as possible. It will be appropriately safeguarded and shared only with those individuals making travel arrangements on your behalf.</p>
            <h3 class="subsection">
                <asp:Label ID="lblNameSubSection" runat="server" Text="General &amp; Passenger Information"></asp:Label></h3>
            <asp:Panel ID="pnlNameSubSection" runat="server">
                <div id="divGeneralandPassenger" runat="server">

                    <div class="row">
                        <div class="col-xs-12 col-md-5">
                            <div id="dvPersonRequesting" runat="server" class="form-group required">
                                <asp:Label ID="lblPersonRequesting" AssociatedControlID="txtNameofPersonRequesting" CssClass="control-label" runat="server" Text="Name of Person Requesting Information"></asp:Label>
                                <asp:TextBox ID="txtNameofPersonRequesting" required="required" CssClass="form-control" runat="server" placeholder="Name of Person Requesting Information"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-md-5">
                            <div id="divGeneralandPassengerEmail" runat="server" class="form-group required">
                                <asp:Label ID="lblGeneralandPassengerEmail" runat="server" AssociatedControlID="txtGeneralandPassengerEmail" CssClass="control-label" Text="Email Address"></asp:Label>
                                <asp:TextBox ID="txtGeneralandPassengerEmail" runat="server" MaxLength="128" type="email" placeholder="Enter your Email Address" required="required" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-12 col-md-5">
                            <div id="divPhoneNumber" runat="server" class="form-group required">
                                <asp:Label ID="lblPhoneNumber" runat="server" AssociatedControlID="txtPhoneNumber" CssClass="control-label" Text="Phone Number"></asp:Label>
                                <asp:TextBox ID="txtPhoneNumber" runat="server" ClientIDMode="Static" CssClass="form-control telephone" data-codeid="#txtPhoneNumber" required="required" placeholder="Mobile Phone XXX-XXX-XXXX" MaxLength="12" type="tel" pattern="\d{3}[\-]\d{3}[\-]\d{4}" title="Must match the format XXX-XXX-XXXX"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
                <p class="col-xs-12 col-md-10 alert alert-info">
                    Please provide passenger names as they appear on government issued photo ID.
                    Depending on your destination you may need a passport that is valid for 6 months after travel.
                </p>

                <%--/********************************************--%>
                <div runat="server" visible="false">
                    <div id="divName" runat="server" visible="true">
                        <div class="row">
                            <div class="col-xs-12 col-md-4">
                                <div class="form-group required">
                                    <asp:Label ID="lblPassengerRequired" CssClass="control-label" AssociatedControlID="txtPassengerFirstName" runat="server" Text="Passenger First Name" />
                                    <asp:TextBox ID="txtPassengerFirstName" required="required" CssClass="form-control" runat="server" placeholder="Enter Passenger First Name"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-xs-12 col-md-2">
                                <div class="form-group">
                                    <asp:Label ID="lblPassengerMiddleName" CssClass="control-label" AssociatedControlID="txtPassengerMiddleName" runat="server" Text="Passenger Middle Name" />
                                    <asp:TextBox ID="txtPassengerMiddleName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Passenger Middle Name"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-xs-12 col-md-4">
                                <div class="form-group required">
                                    <asp:Label ID="lblPassengerNameRequired" CssClass="control-label" AssociatedControlID="txtPassengerLastName" runat="server" Text="Passenger Last Name" />
                                    <asp:TextBox ID="txtPassengerLastName" CssClass="form-control" required="required" runat="server" placeholder="Enter Passenger Last Name"></asp:TextBox>
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
                    <div class="row">
                        <div class="col-xs-12 col-md-5">
                            <div class="form-group">
                                <asp:Label ID="lblPassportNbrText" AssociatedControlID="txtPassportNumber" CssClass="control-label" runat="server" Text="Passport Number"></asp:Label>
                                <asp:TextBox ID="txtPassportNumber" CssClass="form-control" MaxLength="20" runat="server" placeholder="Enter your Passport Number"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-xs-12 col-md-5">
                            <div class="form-group">
                                <div id="divExpirationDate" runat="server">
                                    <asp:Label ID="lblPassportExpDateText" AssociatedControlID="txtExpirationDate" CssClass="control-label" runat="server" Text="Passport Expiration Date"></asp:Label>
                                    <div class="input-group" id="passport-expiration">
                                        <asp:TextBox ID="txtExpirationDate" CssClass="form-control" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                        <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <%--/**************************************--%>
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="col-xs-12 col-md-12">
                            <asp:Repeater ID="rptPersonInfo" runat="server" OnItemDataBound="rptPersonInfo_ItemDataBound">
                                <ItemTemplate>
                                    <div class="person">
                                        <div class="frequent-flier">
                                            <asp:Repeater ID="rptPassangerInfo" runat="server" OnItemDataBound="rptPassangerInfo_ItemDataBound">
                                                <ItemTemplate>
                                                    <div class="row frequent-flier-number">

                                                        <div class="form-group clearfix">
                                                            <div id="divName" runat="server" visible="true">
                                                                <div class="row">
                                                                    <div class="col-xs-12 col-md-4">
                                                                        <div class="form-group required">
                                                                            <asp:Label ID="lblPassengerRequired" CssClass="control-label" AssociatedControlID="txtPassengerFirstName" runat="server" Text="Passenger First Name" />
                                                                            <asp:TextBox ID="txtPassengerFirstName" CssClass="form-control" runat="server" placeholder="Enter Passenger First Name"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-md-2">
                                                                        <div class="form-group">
                                                                            <asp:Label ID="lblPassengerMiddleName" CssClass="control-label" AssociatedControlID="txtPassengerMiddleName" runat="server" Text="Passenger Middle Name" />
                                                                            <asp:TextBox ID="txtPassengerMiddleName" CssClass="form-control" MaxLength="40" runat="server" placeholder="Enter Passenger Middle Name"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                    <div class="col-xs-12 col-md-4">
                                                                        <div class="form-group required">
                                                                            <asp:Label ID="lblPassengerNameRequired" CssClass="control-label" AssociatedControlID="txtPassengerLastName" runat="server" Text="Passenger Last Name" />
                                                                            <asp:TextBox ID="txtPassengerLastName" CssClass="form-control" runat="server" placeholder="Enter Passenger Last Name"></asp:TextBox>
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
                                                                                <asp:TextBox ID="txtDateOfBirth" ClientIDMode="Static" CssClass="form-control" MaxLength="10" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                                                                <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <div id="divDisplayGender" runat="server" class="col-xs-12 col-md-5">
                                                                        <div id="dvGenderRequired" runat="server" class="form-group required">
                                                                            <asp:Label ID="lblGenderText" runat="server" AssociatedControlID="ddlGender" CssClass="control-label" Text="Gender"></asp:Label>
                                                                            <asp:DropDownList ID="ddlGender" CssClass="form-control" runat="server">
                                                                                <asp:ListItem Value="">Select an Option</asp:ListItem>
                                                                                <asp:ListItem Value="Male">Male</asp:ListItem>
                                                                                <asp:ListItem Value="FeMale">Female</asp:ListItem>
                                                                            </asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="row">
                                                                <div class="col-xs-12 col-md-5">
                                                                    <div class="form-group">
                                                                        <asp:Label ID="lblPassportNbrText" AssociatedControlID="txtPassportNumber" CssClass="control-label" runat="server" Text="Passport Number"></asp:Label>
                                                                        <asp:TextBox ID="txtPassportNumber" CssClass="form-control" MaxLength="20" runat="server" placeholder="Enter your Passport Number"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-xs-12 col-md-5">
                                                                    <div class="form-group">
                                                                        <div id="divExpirationDate" runat="server">
                                                                            <asp:Label ID="lblPassportExpDateText" AssociatedControlID="txtExpirationDate" CssClass="control-label" runat="server" Text="Passport Expiration Date"></asp:Label>
                                                                            <div class="input-group" id="passport-expiration">
                                                                                <asp:TextBox ID="txtExpirationDate" CssClass="form-control" runat="server" placeholder="MM/DD/YYYY"></asp:TextBox>
                                                                                <div class="input-group-addon"><span class="fa fa-calendar"></span></div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <div class="row" runat="server" id="divselections">
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <div class="clearfix"></div>
            <asp:Panel ID="pnFrequentFlyerInformation" runat="server">
                <h3 class="subsection">
                    <asp:Label ID="Label9" runat="server" Text="Frequent Flyer Information"></asp:Label></h3>
                <div class="row">
                    <div class="col-xs-12 col-md-5" runat="server" visible="false">
                        <div id="divPreferredCarrier" runat="server" class="form-group">
                            <asp:Label ID="lblddlAirlineCarrier" CssClass="control-label" AssociatedControlID="ddlAirline" runat="server" Text="Airline "></asp:Label>
                            <asp:DropDownList ID="ddlAirline" CssClass="form-control" runat="server">
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
                    </div>

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
                                                            <asp:TextBox ID="txtFrequentFlyerNumber" CssClass="form-control" runat="server" placeholder="Enter your Frequent Flyer Number"></asp:TextBox>
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
                    <asp:Label ID="Label8" runat="server" Text="Trip Information"></asp:Label></h3>

                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="divDepartureCity" runat="server" class="form-group required">
                            <asp:Label ID="Label10" CssClass="control-label" AssociatedControlID="txtDepartureCity" runat="server" Text="Departure City"></asp:Label>
                            <asp:TextBox ID="txtDepartureCity" required="required" CssClass="form-control" runat="server" MaxLength="80" placeholder="Enter your Departure City"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="divPreferredDatesofTravel" runat="server" class="form-group required">
                            <asp:Label ID="Label13" CssClass="control-label" AssociatedControlID="txtPreferredDatesofTravel" runat="server" Text="Preferred Dates of Travel"></asp:Label>
                            <asp:TextBox ID="txtPreferredDatesofTravel" required="required" CssClass="form-control" runat="server" MaxLength="80" placeholder="Enter your Preferred Dates of Travel"></asp:TextBox>
                        </div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="divDestinationsInterestedIn" runat="server" class="form-group required">
                            <asp:Label ID="Label12" CssClass="control-label" AssociatedControlID="txtDestinationsInterestedIn" runat="server" Text="Destinations Interested In"></asp:Label>
                            <asp:TextBox ID="txtDestinationsInterestedIn" required="required" CssClass="form-control" runat="server" placeholder="List the destination(s) you are considering"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="divDestinationsNotInterestedIn" runat="server" class="form-group">
                            <asp:Label ID="Label14" CssClass="control-label" AssociatedControlID="txtDestinationsNotInterestedIn" runat="server" Text="Destinations Not Interested In"></asp:Label>
                            <asp:TextBox ID="txtDestinationsNotInterestedIn" CssClass="form-control" runat="server" placeholder="List any destinations you do not want to consider"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="divPreferredAirline" runat="server" class="form-group">
                            <asp:Label ID="Label11" CssClass="control-label" AssociatedControlID="ddlPreferredAirline" runat="server" Text="Preferred Airline"></asp:Label>
                            <asp:DropDownList ID="ddlPreferredAirline" CssClass="form-control" runat="server">
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
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="divRoomsNeeded" runat="server" class="form-group required">

                            <asp:Label ID="lblRoomsNeeded" CssClass="control-label" AssociatedControlID="ddlRoomsNeeded" runat="server" Text="How many rooms are needed?"></asp:Label>
                            <asp:DropDownList ID="ddlRoomsNeeded" CssClass="form-control" runat="server">
                                <asp:ListItem Value="" Text="Select an Option"></asp:ListItem>
                                <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                <asp:ListItem Value="4" Text="4"></asp:ListItem>

                            </asp:DropDownList>
                        </div>

                    </div>

                </div>
                <div class="row">
                    <div class="col-xs-12 col-md-5">
                        <div id="divImportantAmenities" runat="server" class="form-group">
                            <asp:Label ID="lblImportantAmenities" CssClass="control-label" AssociatedControlID="txtImportantAmenities" runat="server" Text="What hotel amenities are important to you?"></asp:Label>
                            <asp:TextBox ID="txtImportantAmenities" CssClass="form-control" TextMode="MultiLine" runat="server" placeholder="Tell us about the hotel amenities that are important to you"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-xs-12 col-md-5">
                        <div id="div1" runat="server" class="form-group">
                            <asp:Label ID="lblVacationDetails" CssClass="control-label" AssociatedControlID="txtVacationDetails" runat="server" Text="What do you like to do while on vacation?"></asp:Label>
                            <asp:TextBox ID="txtVacationDetails" CssClass="form-control" TextMode="MultiLine" runat="server" placeholder="Tell us a little about what you like to do while on vacation"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <asp:Panel ID="Panel1" runat="server">
                    <h3 class="subsection">
                        <asp:Label ID="Label1" runat="server" Text="CAPTCHA"></asp:Label></h3>

                    <div class="col-xs-12 col-md-12">

                        <div>
                            <strong>To submit, type the characters you see in the picture below</strong>
                            <cc1:CaptchaControl ID="cptCaptcha" runat="server" CaptchaBackgroundNoise="Low" CaptchaLength="5" CaptchaHeight="60" CaptchaWidth="200" CaptchaLineNoise="None" CaptchaMinTimeout="5" CaptchaMaxTimeout="240" FontColor="#529E00"
                                                ErrorInputTooFast="Image text was typed too quickly. " ErrorInputTooSlow="Image text was typed too slowly." EnableViewState="False"/>
                        </div>
                        <br />
                        <div class="col-xs-12 col-md-5 required">
                            <div id="div2" runat="server" class="form-group required">
                                <asp:Label CssClass="control-label" AssociatedControlID="txtCaptcha" runat="server" Text="Characters"> </asp:Label>
                                <asp:TextBox ID="txtCaptcha" CssClass="form-control" runat="server" required="required"></asp:TextBox>
                            </div>
                        </div>
                        <br />

                    </div>
                    
                    <div>&nbsp;</div>
                   
                </asp:Panel>
                <div class="form-group prepend-top">
                    <asp:Button ID="btnSaveAndContinue" runat="server" Text="Submit" CssClass="btn btn-success submit-button" data-loading-text="Please Wait..." OnClick="btnSubmit_Click"></asp:Button>
                </div>
            </asp:Panel>

        </div>
    </div>
    <script src="scripts/guestinformation.js"></script>
    <script src="scripts/formvalidation.js"></script>


</asp:Content>
