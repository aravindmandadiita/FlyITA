using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FlyITA.Diagnostics
{
    public partial class Session : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            session_readout();
        }


        private void session_readout()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table class=\"CSSTableGenerator\"><thead><tr><th>Key</th><th>Value</th></tr></thead><tbody>");

            foreach (string key in HttpContext.Current.Session.Keys)
            {
                sb.AppendLine("<tr><td>" + key + "</td><td>" + HttpContext.Current.Session[key] + "</td></tr>");
            }

            sb.AppendLine("</thead></table>");

            litSession.Text = sb.ToString();
        }
    }
}