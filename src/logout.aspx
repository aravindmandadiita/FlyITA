<%@ Page Title="Registration Logout" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="logout.aspx.cs" Inherits="FlyITA.logout" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CustomHeaders" runat="Server">
    <style> #links1, #links2 { visibility:hidden; } </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Main" runat="server">
    <h1>Thank you<br /><small>You have successfully logged out</small></h1>
    <div class="single-col-text">
        <p>&nbsp;</p><p>&nbsp;</p>
        <h5 class="text-center">
            <asp:Label ID="lblMessage" runat="server" Text="Thank you for visiting. Please check back again."></asp:Label>
        </h5>
        <p class="text-center"><a class="btn btn-primary" href="home.aspx">Home</a></p>
    </div>
    <span id="PreventTimeoutModal"></span>
</asp:Content>
