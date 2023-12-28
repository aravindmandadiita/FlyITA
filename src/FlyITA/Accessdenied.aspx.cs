using System;

namespace FlyITA
{
    public partial class Accessdenied : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Abandon();
        }
    }
}