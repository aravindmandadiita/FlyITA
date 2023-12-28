using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class ThankYou : PageBase
    {


        #region " ### Page Events ### "

        private void Page_PreInit(object sender, System.EventArgs e)
        {

        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Request.QueryString["page"] == "Vacation")
            {
                lblNonRegHeader.Text = "Vacation Travel <br /><small>Vacation Travel Request</small>";
                HText.InnerText = "Vacation Travel Request submitted successfully";

            }

        }
        #region " ### Public Overridden Methods ### "

        public override Label UserMessages
        {
            get { return this.lblInfo; }
            set { this.lblInfo.Text = value.Text; }
        }

        #endregion



        #endregion


    }

}