<%@ Page Title="Home" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" EnableSessionState="true" CodeBehind="Home.aspx.cs" Inherits="FlyITA.Home" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CustomHeaders" runat="server">
    <style>
        body #wrapper #banner-home {
            display: block;
        }

        body #wrapper #banner-site {
            display: none;
        }

        body #wrapper #banner-register {
            display: none;
        }
    </style>
     <script type="text/javascript">
        function viewItinerarydetails() {
            var txtreservationNumber = document.getElementById("<%= txtReservationNum.ClientID%>").value;
            var txttravelerLastName = document.getElementById("<%= txtTravelerFirstName.ClientID%>").value;

            var url = 'https://viewtrip.travelport.com/#!/itinerary?loc=' + txtreservationNumber.trim() + '&lName=' + txttravelerLastName.trim();

            window.open(url);
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Main" runat="Server">
    <div id="homepage" class="clearfix">
        <section id="itinerary" class="row-dark">
            <div class="container"><div class="row">
                <div class="col-md-4">
                    <h3>View Itinerary & Flight Status</h3>
                </div>
                <div class="col-sm-6 col-md-3">
                    <asp:TextBox ID="txtReservationNum" required="required" runat="server" placeholder="Enter Reservation Number"></asp:TextBox>
                </div>
                <div class="col-sm-6 col-md-3">
                    <asp:TextBox ID="txtTravelerFirstName" required="required" runat="server" placeholder="Enter Traveler Last Name"></asp:TextBox>
                </div>
                <div class="col-sm-6 col-md-2">
                    <a class="btn btn-md btn-primary" href="#" id="viewItinerary" onclick="viewItinerarydetails()" runat="server">View Itinerary</a>
                </div>
            </div></div>
        </section>
        <section id="introduction" class="row-light">
            <div class="single-col-text text-center">
                <h2>Travel with Confidence</h2>
                <p class="intro">Today's  business travelers need more than airline tickets, hotel reservations and car  rentals. They need the support of a full-service travel management company—a reliable  business partner with the people, resources and technology to guarantee success.  They need International Travel Associates.</p>
                <p class="intro">ITA has developed this site with you in mind. We offer a web-based booking site  and mobile app that are ready to help you book your flight online—easily and  securely.</p>
                <p><a class="btn btn-md btn-primary" href="reservations.aspx">Learn More</a></p>
            </div>
        </section>
        <section id="vacation" class="row-gray">
            <div class="single-col-text text-center">
                <h2>We know you don't always travel for business</h2>
                <p class="intro">Sometimes, you get to relax and do  the things you enjoy most. Whether you like to play in the snow or the sand or  be on slopes, the green or the sea, we can help.</p>
                <p><a class="btn btn-md btn-primary" href="vacation.aspx">Learn More</a></p>
            </div>
        </section>
        <section id="info-links" class="row-light">
            <div class="container">
                <div class="col-md-4">
                    <h3 class="icon1">Flight check-in<br />& Mileage Programs</h3>
                    <div class="icon-content">
                        <p>Select an airline to check in with:</p>
                        <ul>
                            <li><a target="_blank" href="https://www.aa.com/reservation/flightCheckInViewReservationsAccess.do">American Airlines</a></li>
                            <li><a target="_blank" href="https://www.delta.com/PCCOciWeb/findBy.action">Delta</a></li>
                            <li><a target="_blank" href="https://www.united.com/travel/checkin/start.aspx">United Airlines</a></li>
                        </ul>
                        <p>Airline Mileage Programs:</p>
                        <ul>
                            <li><a target="_blank" href="https://www.aa.com/i18n/AAdvantage/index.jsp">American Airlines</a></li>
                            <li><a target="_blank" href="http://www.delta.com/content/www/en_US/skymiles.html">Delta</a></li>
                            <li><a target="_blank" href="https://www.united.com/web/en-US/content/mileageplus/Default.aspx">United Airlines</a></li>
                        </ul>
                    </div>
                </div>
                <div class="col-md-4">
                    <h3 class="icon2">Travel Updates<br />& Alerts</h3>
                    <div class="icon-content">
                        <p><a href="https://www.viewtrip.com/VTHome.aspx" target="_blank">ViewTrip</a> - ViewTrip  is monitored 24/7 to keep you current on what&rsquo;s happening in your destination  city as well as your home city.</p>
                        <p><a href="https://weather.com/" target="_blank">Weather.com</a> - Visit weather.com to find weather information for anywhere in the world.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <h3 class="icon3">Traveling<br />Internationally</h3>
                    <div class="icon-content">
                        <ul>
                            <li><a href="international.aspx#passport">Passports/Visas</a></li>
                            <li><a href="international.aspx#car">Renting a Car</a></li>
                            <li><a href="international.aspx#security">Security Issues</a></li>
                            <li><a href="international.aspx#cc">Credit Cards and Currency</a></li>
                            <li><a href="international.aspx#cell">Cell Phones</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </section>
        <section id="giftcards" class="row-gray">
            <div class="single-col-text text-center">
                <h2>Give Them the World!</h2>
                <p>
                    What better  gift can you give someone than the entire world? We have gift certificates that  make excellent
        retirement, graduation or wedding gifts. Come to think of it,  whatever the occasion, who wouldn&rsquo;t
        want a gift that gives them unlimited travel  possibilities? Let their imagination run wild! Call (800) 622-4099
        or email <a href="mailto:leisuretravel@itagroup.com">leisuretravel@itagroup.com</a> for more information about
        our gift certificate program.
                </p>

            </div>
        </section>
    </div>

    <script src="scripts/home.js"></script>
</asp:Content>
