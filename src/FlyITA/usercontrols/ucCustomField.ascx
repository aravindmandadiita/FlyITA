<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucCustomField.ascx.cs" Inherits="FlyITA.ucCustomField" %>

<div class="row">
    <div class="col-xs-12 col-md-4">
        <div id="dvRequiredField" runat="server" class="form-group required">
            <div id="dvFieldLabel" runat="server">
                <asp:Label ID="lblCF" CssClass="control-label" runat="server" Text=""></asp:Label>
            </div>
            <div id="dvFieldValue" runat="server">
                <asp:TextBox ID="txtCF" required="required" CssClass="form-control" runat="server" Text=""></asp:TextBox>
                <asp:DropDownList ID="ddlCF" required="required" CssClass="form-control" runat="server"></asp:DropDownList>
                <asp:Label ID="lblCFReadOnly" runat="server" Text="" Visible="false"></asp:Label>
            </div>
        </div>
    </div>
</div>