using System.Linq;
using ITALib;
using ITALib.DataAccess;
using ITALib.Functions;
using ITALogging;
using PCentralLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace FlyITA
{
    public class Global : System.Web.HttpApplication
    {
        public static string error_message_500
        {
            get { return "The system is experiencing an unexpected error. Please try again later or contact Travel Headquarters."; }
        }

        public static string error_message_404
        {
            get { return "The page you're looking for does not exist on our server."; }
        }

        #region error module events ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Fires when the error handler module encounters a 404 error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        //public void ITAErrorHandlerModule_PageNotFound(object sender, EventArgs e)
        //{
        //    var args = (PageNotFoundArgs)e;
        //    if (args.referer == null) // probably typed url wrong
        //    {
        //        args.clear_error = true;

        //        Server.ClearError();
        //        Utilities.Redirect("error.aspx?syserror=E404");
        //    }
        //    // else treat as normal exception
        //}

        /// <summary>
        /// Fires when the error handler module is about to log and/or email an error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ITAErrorHandlerModule_LogException(object sender, EventArgs e)
        {
            string log_id = string.Empty;
            string connection = Utilities.GetEnvironmentConnection();
            bool client_facing = Utilities.IsClientFacingEnv();
            LogEventArgs args = (LogEventArgs)e;
            DataRow dr = null;

            args.log_to_db = false; // cancel the normal log to database action, we're gonna do it right here instead.

            if (Request.Headers["System_Error_Message"] != null)
            {
                // there is probably an error on page load...
                args.suppress_reporting = true;
                args.clear_error = true;

                Server.ClearError();
                Utilities.Redirect("error.aspx?syserror=E500");

                return;
            }

            if (!client_facing)
            {
                args.email = false;
                args.display_full_error = true;
                //return;
            }

            Request.Headers.Add("System_Error_Message", Global.error_message_500);

            if (string.IsNullOrEmpty(connection))
            {
                args.report.additional_info = "Missing Logging Connection";
            }
            else
            {
                try
                {
                    using (DB mssql = new DB(connection))
                    {
                        int priority = 926;
                        string str = args.report.full_report.Replace(Environment.NewLine, Environment.NewLine + "`");
                        const int chunkSize = 4090;
                        IEnumerable<string> errMsg = Enumerable.Range(0, str.Length / chunkSize).Select(i => str.Substring(i * chunkSize, chunkSize));


                        if (args.severity == Logger.Severity.business_error)
                        {
                            priority = (int)Enumerations.enumLogPriority.InfoLevel;
                        }
                        else if (args.severity == Logger.Severity.trace)
                        {
                            priority = (int)Enumerations.enumLogPriority.DebugLevel;
                        }

                        foreach (string err in errMsg)
                        {
                            // log to file
                            List<SqlParameter> parameters = new List<SqlParameter>(9);
                            parameters.Add(DB.MakeIntParam("@SystemID", (int)Enumerations.enumInterfacingSystems.PerformanceCentral));
                            parameters.Add(DB.MakeIntParam("@errorCd", 0));
                            //parameters.Add(DB.MakeVarCharParam("@message", args.report.full_report.Replace(Environment.NewLine, Environment.NewLine + "`").Truncate(4096), 4096));
                            // parameters.Add(DB.MakeVarCharParam("@message", args.report.stack_trace.Truncate(4096), 4096));
                            parameters.Add(DB.MakeVarCharParam("@message", err));
                            parameters.Add(DB.MakeIntParam("@Priority", priority));
                            parameters.Add(DB.MakeVarCharParam("@SourceAssemblyName", "FlyITA"));
                            parameters.Add(DB.MakeVarCharParam("@SourceMethodName", args.report.method_name));
                            parameters.Add(DB.MakeVarCharParam("@SourceClassName", args.report.class_name));
                            parameters.Add(DB.MakeIntParam("@RealUserID", Math.Max(ContextManager.RealUserID, 0)));
                            //parameters.Add(DB.MakeBitParam("@return_matches", true)); defaults to true
                            parameters.Add(DB.MakeReturnParam());

                            dr = mssql.Get_Row("spInsLogMessage", parameters);

                            if (dr != null)
                            {
                                args.stats_returned = true;
                                args.last_five_minutes = dr["last_five_min"].toInt();
                                args.last_half_hour = dr["last_half_hour"].toInt();
                                args.last_hour = dr["last_hour"].toInt();
                                args.last_two_hours = dr["last_two_hours"].toInt();
                                log_id = dr["log_id"].ToString();
                            }
                            else
                            {
                                log_id = mssql.last_action.ReturnParamValue.ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log_id = "Log Error: " + ex.Message;
                }

                if (!string.IsNullOrEmpty(log_id))
                {
                    args.log_id = log_id;
                }
            }

            if (args.severity == Logger.Severity.trace)
            {
                args.email = false;
            }

            args.clear_error = true;
            Server.ClearError();

            // try to land them on the same page that triggered the error...
            //Server.Transfer(Request.RawUrl, false);

            if (!client_facing)
            {
                // try to land them on the same page that triggered the error...
                //Server.Transfer(Request.RawUrl, false);    
            }
            else
            {
                Utilities.Redirect("error.aspx?syserror=E500");
            }

        }

        #endregion

        protected void Application_Start(object sender, EventArgs e)
        {
            // Fires when the application is started
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Fires when the session is started
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Fires at the beginning of each request
            int lngPosition = 0;
            string strFileExtension = string.Empty;
            System.IO.DirectoryInfo dir = default(System.IO.DirectoryInfo);

            lngPosition = Request.Path.IndexOf(".");

            if (lngPosition > -1)
                strFileExtension = Request.Path.Substring(lngPosition);
            if (string.Compare(strFileExtension, ".aspx", true) == 0 || string.Compare(strFileExtension, ".html", true) == 0 || string.Compare(strFileExtension, ".htm", true) == 0)
            {
                dir = new System.IO.DirectoryInfo(Request.PhysicalApplicationPath);

                if (dir.GetFiles("down.html").Length > 0)
                {
                    Response.WriteFile(dir.GetFiles("down.html")[0].FullName);
                    Response.End();
                }
                else if (dir.Parent.GetFiles("down.html").Length > 0)
                {
                    Response.WriteFile(dir.Parent.GetFiles("down.html")[0].FullName);
                    Response.End();
                }
                else if (dir.Parent.Parent.GetFiles("down.html").Length > 0)
                {
                    Response.WriteFile(dir.Parent.Parent.GetFiles("down.html")[0].FullName);
                    Response.End();
                }
            }
            string path = HttpContext.Current.Request.RawUrl;
            //if (path.ToLower() == "/achpayment/") HttpContext.Current.Response.Redirect("~/achpayment.aspx");
        }

        protected void ApplicationException(object sender, EventArgs e)
        {


        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Fires upon attempting to authenticate the user
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Fires when the session ends
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Fires when the application ends
        }
    }
}