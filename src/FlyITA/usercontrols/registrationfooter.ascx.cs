using System;
using PCentralLib;
using PCentralLib.programs;
using System.Text;
using System.Web;

namespace FlyITA.usercontrols
{
    public partial class registrationfooter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadCommunicationFooter();
            }
        }

        public void LoadCommunicationFooter()
        {
           
            StringBuilder sb = new StringBuilder();

            
                sb.Append("Questions? Please contact ");
                sb.Append("<< >>");
                sb.Append(" at ");
                sb.Append("<< >>");
                sb.Append(" or ");
                sb.Append("<< >>");
            

            lblCommunication.Text = sb.ToString();
        }

        private string GetITAProgNbr()
        {
            string ITAProgNbr = Utilities.ReadCustomConfigValue("ITAProgNbr", "value", string.Empty);

            if (string.IsNullOrWhiteSpace(ITAProgNbr))
            {
                ITAProgNbr = HttpContext.Current.Request.Path.Split(new[] { '/' })[1];
            }

            return ITAProgNbr;
        }
    }
}