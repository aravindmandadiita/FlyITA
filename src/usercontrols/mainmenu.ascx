<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mainmenu.ascx.cs" Inherits="FlyITA.usercontrols.mainmenu" %>

    <div class="logo" id="globallogo" runat="server">
        <img class="l-white" src="images/logo-white.png" />
        <img class="l-color" src="images/logo-color.png" />
    </div>
    <nav id="globalmenu" class="navigation" runat="server">
        <ul>
            <li><a href="home.aspx">Home</a></li>
            <li><a href="aboutita.aspx">About ITA</a></li>
            <li class="nav-has-sub">
                <a>Travel Reservations</a>
                <ul class="nav-dropdown">
                    <li><a href="reservations.aspx">Booking Information</a></li>
		            <li><a href="Travelerprofileinformation.aspx">Traveler Profile</a></li>
		            <li>
                        <button type="button" class="btn-menu" data-toggle="modal" data-target="#modalItinerary">View Itinerary and Flight Status</button>
		            </li>
                </ul>
            </li>
            <li class="nav-has-sub">
                <a>Resources</a>
                <ul class="nav-dropdown">
                    <li><a href="resources.aspx">Travel Updates</a></li>
		            <li><a href="international.aspx">International Travel</a></li>
		            <li><a href="travelinfo.aspx">Travel Best Practices</a></li>
		            <li><a href="programs.aspx">Airline Programs</a></li>
                </ul>
            </li>
            <li><a href="vacation.aspx">Vacation Travel</a>
                 
            </li>
            <li><a href="helpsite.aspx"><span class="glyphicon glyphicon-envelope"></span></a></li>
        </ul>
    </nav>
