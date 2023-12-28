using PCentralLib.WebReg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA.Diagnostics
{
    public partial class PartyEventsInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataSet EventDetails = PCentralWebRegDAL.ReadEventDetailsByPartyID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

            litDump.Text = "<h3>List of auto-assigned events the party is already enrolled in automatically...</h3>" + Environment.NewLine;
            litDump.Text += ITALib.DataAccess.DB_Extensions.toHTML(EventDetails.Tables[0], "class=\"CSSTableGenerator\"");

            litDump.Text += "<h3>List of event categories...</h3>" + Environment.NewLine;
            litDump.Text += ITALib.DataAccess.DB_Extensions.toHTML(EventDetails.Tables[1], "class=\"CSSTableGenerator\"");

            litDump.Text += "<h3>List of event options for each event category...</h3>" + Environment.NewLine;
            litDump.Text += ITALib.DataAccess.DB_Extensions.toHTML(EventDetails.Tables[2], "class=\"CSSTableGenerator\"");

            litDump.Text += "<h3>List of auto-assigned fees for each event...</h3>" + Environment.NewLine;
            litDump.Text += ITALib.DataAccess.DB_Extensions.toHTML(EventDetails.Tables[3], "class=\"CSSTableGenerator\"");

            litDump.Text += "<h3>List of fees currently assigned to the party...</h3>" + Environment.NewLine;
            litDump.Text += ITALib.DataAccess.DB_Extensions.toHTML(EventDetails.Tables[4], "class=\"CSSTableGenerator\"");
        }
    }
}