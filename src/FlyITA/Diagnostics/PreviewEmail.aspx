<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreviewEmail.aspx.cs" Inherits="FlyITA.Diagnostics.PreviewEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Preview Email</title>
    
    <style type="text/css">

        body {
	        font-family: Verdana, Geneva, sans-serif;
	        font-size: 70%;
	        color: #000000;
	        letter-spacing: 0px;
	        line-height: 1.2em;
	        font-weight: normal;
	        font-style: normal;
	        text-transform: none;
        }

        .button-link {
            padding: 10px 15px;
            background: #4479BA;
            color: #FFF;
            -webkit-border-radius: 4px;
            -moz-border-radius: 4px;
            border-radius: 4px;
            border: solid 1px #20538D;
            text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.4);
            -webkit-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
            -moz-box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
            box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.4), 0 1px 1px rgba(0, 0, 0, 0.2);
            -webkit-transition-duration: 0.2s;
            -moz-transition-duration: 0.2s;
            transition-duration: 0.2s;
            -webkit-user-select:none;
            -moz-user-select:none;
            -ms-user-select:none;
            user-select:none;
            text-decoration:none;
            line-height:50px;
            font-family: Verdana, Geneva, sans-serif;
            font-weight:bold;
            font-size:11px;
        }
        .button-link:hover {
            background: #356094;
            border: solid 1px #2A4E77;
            text-decoration: none;
        }
        .button-link:active {
            -webkit-box-shadow: inset 0 1px 4px rgba(0, 0, 0, 0.6);
            -moz-box-shadow: inset 0 1px 4px rgba(0, 0, 0, 0.6);
            box-shadow: inset 0 1px 4px rgba(0, 0, 0, 0.6);
            background: #2E5481;
            border: solid 1px #203E5F;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
    
        <a href="Default.aspx" class="button-link">Diagnostics Menu</a>
        <br />
        <b>Email Type:</b>
        <asp:DropDownList runat="server" ID="ddlEmails" AutoPostBack="false">
            <asp:ListItem Value="Self-Enroll" />
        </asp:DropDownList>

        <hr />

        <asp:Literal ID="litEmail" runat="server" Mode="PassThrough" EnableViewState="false" />

    </form>
</body>
</html>
