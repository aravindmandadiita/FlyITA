using PCentralLib.correspondence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ITALib;
using PCentralLib.participants;
using PCentralLib;

namespace FlyITA.Diagnostics
{
    public partial class PreviewEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ddlEmails.SelectedIndex == 0)
            {
                PreviewSelfEnrollEmail();
            }
        }

        private void PreviewSelfEnrollEmail()
        {
            PCentralParticipant participant = PCentralParticipant.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ParticipantID);

            PCentralCorrespondence Correspondence = null;

            // Send custom logon credentials email
            if (Utilities.ReadCustomConfigValue("pages/selfenroll.aspx", "option_send_login_credentials").toBool(false))
            {
                CustomEmails customEmail = new CustomEmails();
                litEmail.Text = customEmail.PreviewLogonCredentials(null);
                return;
            }
            else if (participant.BelongsTo_Program.UseSystemLogonCredentialsFlg)
            {
                // send normal p-central email

                
            }
            else
            {
                // Email some other crap?
                /*
                Correspondence = new PCentralCorrespondence(Utilities.GetEnvironmentConnection())
                {
                    CorrespondenceType = Enumerations.enumCorrespondenceTypes.LogonNotification,
                    ProgramId = ContextManager.ProgramID,
                    PersonId = ContextManager.PersonID
                };
                //*/
            }

            if (Correspondence != null)
            {
                litEmail.Text = PCentralCorresponenceFunctions.PreviewParticipantCorrespondence(Correspondence, ContextManager.ParticipantID, ContextManager.RealUserID, Guid.NewGuid());
            }
        }
    }
}