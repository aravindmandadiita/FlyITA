<%@ Page Title="Flight Check-In and Mileage Programs" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="programs.aspx.cs" Inherits="FlyITA.programs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CustomHeaders" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
<h1>Airline Programs<br />
  <small>Flight Check-In and Mileage</small></h1>
<div class="row-light">
  <div class="single-col-text">
    <h3>Online Check-in</h3>
      <p>Use the links below to check in for your  flight.</p>
      <ul>
        <li><a href="https://www.aa.com/aa/reservation/flightCheckInViewReservationsAccess.do" target="_blank">American  Airlines </a></li>
        <li><a href="http://delta.com/traveling_checkin/index.jsp" target="_blank">Delta</a></li>
        <li><a href="http://www.united.com/checkin" target="_blank">United Airlines</a></li></ul>
        <h3>Airline Mileage Programs</h3>
        <ul>
          <li><strong>American Airlines:</strong> <a href="https://www.aa.com/i18n/AAdvantage/index.jsp" target="_blank">AAdvantage Program</a></li>
          <li><strong>Delta:</strong> <a href="https://www.delta.com/skymilesenrollment/skymiles/enrollment/index.jsp" target="_blank">Skymiles<sup>&reg;</sup></a></li>
           <li><strong>United Airlines</strong>: <a href="https://www.united.com/web/en-US/content/mileageplus/default.aspx" target="_blank">Mileage Plus<sup>&reg;</sup></a></li>
        </ul>
  </div>
</div>

</asp:Content>
