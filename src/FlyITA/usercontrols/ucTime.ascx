<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucTime.ascx.cs" Inherits="FlyITA.usercontrols.ucTime" %>
<asp:TextBox ID="ITATimeCtrlHr" MaxLength="2" CssClass="xsm" runat="server"></asp:TextBox>&nbsp;:
<asp:TextBox ID="ITATimeCtrlMn" MaxLength="2" CssClass="xsm" runat="server"></asp:TextBox>
<asp:DropDownList ID="ITATimeCtrlTimeType" CssClass="xsm" runat="server">
    <asp:ListItem Value="0">Select</asp:ListItem>
    <asp:ListItem Value="AM">a.m.</asp:ListItem>
    <asp:ListItem Value="PM">p.m.</asp:ListItem>
</asp:DropDownList>
