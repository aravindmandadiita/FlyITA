using PCentralLib.participants;
using PCentralLib.parties;
using PCentralLib.programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class DumpProg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PCentralProgram party = PCentralProgram.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID);

            litDump.Text = party.DumpToHTML(true, true);
        }
    }
}