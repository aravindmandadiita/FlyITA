<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DumpUser.aspx.cs" Inherits="FlyITA.DumpUser" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Party Dump</title>

    <style type="text/css">

        #paxdump {
	        font-family: Verdana, Geneva, sans-serif;
	        font-size: 70%;
	        color: #000000;
	        letter-spacing: 0px;
	        line-height: 1.2em;
	        font-weight: normal;
	        font-style: normal;
	        text-transform: none;
        }

        #paxdump table {
            border-collapse: collapse;
        }

        #paxdump th {
            padding:2px;
            background-color: #0066FF;
            color: #FFF;
        }

        #paxdump td.dump_label,
        #paxdump td.dump_data {
            padding:4px;
            vertical-align:top;
            border-bottom:1px solid #79b0c8;
        }

        #paxdump td.dump_label {
            text-align:right;
            background-color: #99CCFF;
        }

        #paxdump td.dump_data {
            text-align:left;
            background-color: #FFF;
        }

        /* level 2 */
        
        #paxdump td.dump_data td.dump_label {
            background-color: #ebc786;
        }

        #paxdump td.dump_data th {
            background-color: #644919;
        }

        /* level 3 */
        
        #paxdump td.dump_data td.dump_data td.dump_label {
            background-color: #79c38f;
        }

        #paxdump td.dump_data td.dump_data th {
            background-color: #235a34;
        }

        /* level 4 */
        
        #paxdump td.dump_data td.dump_data td.dump_data td.dump_label {
            background-color: #d8acac;
        }

        #paxdump td.dump_data td.dump_data td.dump_data th {
            background-color: #700b0b;
        }

        /* level 5 */
        
        #paxdump td.dump_data td.dump_data td.dump_data td.dump_data td.dump_label {
            background-color: #b2c379;
        }

        #paxdump td.dump_data td.dump_data td.dump_data td.dump_data th {
            background-color: #3f4824;
        }

        /* level 6 */
        
        #paxdump td.dump_data td.dump_data td.dump_data td.dump_data td.dump_data td.dump_label {
            background-color: #BBBBBB;
        }

        #paxdump td.dump_data td.dump_data td.dump_data td.dump_data td.dump_data th {
            background-color: #393939;
        }

        .empty {
            color:#999;
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
    <div id="paxdump">
        <asp:Literal ID="litDump" runat="server" Mode="PassThrough" EnableViewState="false" />
    </div>
    </form>
</body>
</html>
