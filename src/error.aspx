<%@ Page Title="" Language="C#" MasterPageFile="~/_Main.Master" AutoEventWireup="true" CodeBehind="error.aspx.cs" Inherits="FlyITA.error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomHeaders" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" runat="server">
    <p class="text-danger text-center"><asp:Label ID="lblInfo" runat="server"></asp:Label></p>
    <div class="text-center">
        <a href="Logout.aspx">Log Out</a>
    </div>
</asp:Content>
