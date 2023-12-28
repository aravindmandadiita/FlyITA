<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PersonMatch.aspx.cs" Inherits="FlyITA.Diagnostics.PersonMatch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Person Matching</title>
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
            padding:20px;
        }

        strong
        {
            font-weight:bold;
	        font-size: 15px;
	        color: #ce0000;
        }

        div.ctrls
        {
            border:2px solid #999;
            width:550px;
            display:flexbox;
            padding:15px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            border-radius: 10px;
            margin-top:10px;
            background-color:#DDDDDD;
        }

        div.ctrls table
        {
            border-collapse: collapse;
            width:100%;
            margin-bottom:10px;
        }

        div.ctrls table tr td
        {
            padding:4px 5px;
        }

        div.ctrls table tr td.label{
            font-weight:bold;
            text-align:center;
        }

        div.ctrls table tr td.label_right{
            font-weight:bold;
            text-align:right;
        }

        .CSSTableGenerator {
            margin: 0px;
            padding: 0px;
            width: 100%;
            border: 1px solid #000000;
            margin-bottom: 20px;
            border-collapse: collapse;
        }

        .CSSTableGenerator tr:nth-child(odd) { background-color:#e5e5e5; }
        .CSSTableGenerator tr:nth-child(even) { background-color:#ffffff; }
        .CSSTableGenerator td, .CSSTableGenerator thead tr th {
	        vertical-align:middle;
	        border:1px solid #000000;
	        border-width:0px 1px 1px 0px;
	        text-align:left;
	        padding:5px;
	        color:#000000;
        }

        .CSSTableGenerator thead tr:first-child th {
		    background:-o-linear-gradient(bottom, #4479ba 5%, #2e5481 100%);	
            background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #4479ba), color-stop(1, #2e5481) );
	        background:-moz-linear-gradient( center top, #4479ba 5%, #2e5481 100% );
	        filter:progid:DXImageTransform.Microsoft.gradient(startColorstr="#4479ba", endColorstr="#2e5481");	
            background: -o-linear-gradient(top,#4479ba,2e5481);
	        background-color:#4479ba;
	        border:0px solid #000000;
	        text-align:center;
	        border-width:0px 0px 1px 1px;
	        font-size:11px;
	        font-weight:bold;
	        color:#ffffff;
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

        <hr />

    <div class="ctrls">
    <table>
        <tr>
            <td class="label">Prefix</td>
            <td class="label">First</td>
            <td class="label">Middle</td>
            <td class="label">Last</td>
            <td class="label">Suffix</td>
        </tr>
        <tr>
            <td><asp:DropDownList ID="ddlPrefix" runat="server" /></td>
            <td><asp:TextBox ID="txtLegalFirstName" MaxLength="40" runat="server" /> </td>
            <td><asp:TextBox ID="txtLegalMiddleName" MaxLength="40" runat="server" Width="40" /></td>
            <td><asp:TextBox ID="txtLegalLastName" MaxLength="40" runat="server" /></td>
            <td><asp:DropDownList ID="ddlSuffix" CssClass="sm" runat="server" /></td>
        </tr>
    </table>
    <table>
        <tr>
            <td class="label_right">Birth Date</td><td><asp:TextBox ID="txtBirthDate" runat="server" /></td>
            <td class="label_right">Gender</td><td><asp:DropDownList ID="ddlGender" runat="server" /></td>
        </tr>
        <tr>
            <td class="label_right">EMail</td><td><asp:TextBox ID="txtEmail" runat="server" /></td>
            <td class="label_right">CPI</td><td><asp:TextBox ID="txtClientProvidedID" runat="server" /></td>
        </tr>
    </table>

    <asp:Button ID="btnGetMatches" runat="server" OnClick="btnGetMatches_Click" Text="Show Matches" CssClass="button-link" />
    </div>

    <br /><br />

    <asp:Literal ID="litMatches" runat="server" Mode="PassThrough" EnableViewState="false" />

    </form>
</body>
</html>
