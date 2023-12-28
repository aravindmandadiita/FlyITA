using System;
using System.Text;
using PCentralLib.programs;

namespace FlyITA
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var program = PCentralProgram.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID.ToString());

            Session.Abandon();

            if (Utilities.IsNothingNullOrEmpty(Request.QueryString["msg"])) return;
            switch (Request.QueryString["msg"].ToLower())
            {
                case "locked":
                    var sb = new StringBuilder();

                    sb.Append("You are unable to complete your registration online due to your account being locked. Please contact ");
                    sb.Append(program.TravelHeadquartersName);
                    sb.Append(" at ");
                    sb.Append(program.ProgramTollFreeNbr);
                    sb.Append(" to speak to a representative.");

                    this.lblMessage.Text = sb.ToString();
                    break;
                case "unknownpax":
                    this.lblMessage.Text = "Login is not valid for this site program.";
                    break;
                case "unknownprog":
                    this.lblMessage.Text = "Program is not known.";
                    break;
                case "timeout":
                    this.lblMessage.Text = "For your security, your session has timed out due to inactivity.  You may log in again at any time to continue using the registration site.";
                    break;
            }
        }
    }
}