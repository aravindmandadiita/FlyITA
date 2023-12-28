using ITALib;
using System;
using System.Configuration;

namespace FlyITA
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 

 
            Utilities.Redirect("Home.aspx");
        }
    }
}