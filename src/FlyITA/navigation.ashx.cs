using System;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace FlyITA
{
    /// <summary>
    /// Summary description for eventsjson
    /// </summary>
    public class Navigation : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            Utilities.SetNextPage(context.Request.QueryString["target"]);
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"Success\":true}");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}