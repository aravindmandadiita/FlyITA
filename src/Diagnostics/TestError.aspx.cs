using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA.Diagnostics
{
    public partial class TestError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!show_system_error())
            {
                throw new Exception("This is only a web-reg test!");
            }
        }

        public bool show_system_error()
        {
            string err = HttpContext.Current.Request.Headers["System_Error_Message"] ?? "";
            if (!string.IsNullOrWhiteSpace(err))
            {
                this.lblInfo.Text += err;
                return true;
            }
            else if (HttpContext.Current.Request.Params["syserror"] != null)
            {
                this.lblInfo.Text += Global.error_message_500;
                return true;
            }
            return false;
        }
    }
}