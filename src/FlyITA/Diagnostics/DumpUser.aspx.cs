using PCentralLib.participants;
using PCentralLib.parties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class DumpUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PCentralParty party = PCentralParty.GetByParticipantID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

            litDump.Text = party.DumpToHTML(true, true);
        }
    }
}