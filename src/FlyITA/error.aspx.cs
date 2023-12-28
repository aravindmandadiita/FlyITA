using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class error : System.Web.UI.Page
    {
        protected void Page_Load(System.Object sender, System.EventArgs e)
        {
            if (Request.Params["syserror"] != null)
            {
                switch (Request.Params["syserror"].ToString())
                {
                    case "E404": this.lblInfo.Text = Global.error_message_404; return;
                }
            }

            this.lblInfo.Text = Global.error_message_500;
        }
    }
}