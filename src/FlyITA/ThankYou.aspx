<%@ Page Title="Registration - Thank You" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="ThankYou.aspx.cs" Inherits="FlyITA.ThankYou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomHeaders" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">

    <h1>
        <asp:Label ID="lblNonRegHeader" runat="server" Text="Traveler Profile <br /><small>Traveler Information</small>"></asp:Label>


    </h1>
    <div class="single-col-text">
        <p class="text-danger">
            <asp:Label ID="lblInfo" runat="server"></asp:Label>
        </p>
        <p class="center">
            <h2 runat="server" id="HText">Traveler profile submitted successfully.</h2>

        </p>
        <p>
        </p>


    </div>
</asp:Content>
