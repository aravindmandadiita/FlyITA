#region Includes

using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using ITALib;
using ITALib.Configuration;
using ITALib.DataAccess;
using PCentralLib;
using PCentralLib.custom_fields;
using PCentralLib.logging;
using PCentralLib.participants;
using PCentralLib.parties;
using PCentralLib.programs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using FlyITA;

#endregion

/// <summary>
/// contains shared (static) functions for use throughout site
/// </summary>
public static class Utilities
{
    #region Navigation & Navigation.config access

    public static Hashtable ReadNavConfigValues(string Section)
    {
        string nspath = ConfigurationManager.AppSettings["NavSettingsPath"].toStringOr("configuration/Navigation.config");

        //--- returns a hashtable of values for the specified section of the custom config file
        return GetCustomConfig(HttpContext.Current.Server.MapPath(nspath), Section);
    }

    public static bool preview_redirects
    {
        get { return false; }
    }

    public static void SetNextPage(string nextPage)
    {
        if (nextPage.Contains("?"))
        {
            HttpContext.Current.Session["NextPage"] = nextPage.Substring(0, nextPage.IndexOf("?"));
        }
        else
        {
            HttpContext.Current.Session["NextPage"] = nextPage;
        }
    }

    public static void Navigate(string nextPage, string from_where = "")
    {
        //--- redirects to NextPage, first setting the session to allow for navigation enforcement
        //--- within the Master Page_Init event
        if (nextPage.Contains("?"))
        {
            HttpContext.Current.Session["NextPage"] = nextPage.Substring(0, nextPage.IndexOf("?"));
        }
        else
        {
            HttpContext.Current.Session["NextPage"] = nextPage;
        }

        if (Utilities.preview_redirects)
        {
            Page page = HttpContext.Current.Handler as Page;
            _Main master = (_Main)(page.Master);
            master.redirect_to(nextPage, from_where);

            //if (page.Master.GetType().ToString() == "ASP._login_master")
            //{
            //    _Login master = (_Login)(page.Master);
            //    master.redirect_to(nextPage, from_where);
            //}
            //else
            //{
            //    _Main master = (_Main)(page.Master);
            //    master.redirect_to(nextPage, from_where);
            //}
        }
        else
        {
            HttpContext.Current.Response.Redirect(nextPage);
        }
    }

    public static void Redirect(string location, string from_where = "")
    {
        if (Utilities.preview_redirects)
        {
            Page page = HttpContext.Current.Handler as Page;
            _Main master = (_Main)(page.Master);
            master.redirect_to(location, from_where);

            //if (page.Master.GetType().ToString() == "ASP._login_master")
            //{
            //    _Login master = (_Login)(page.Master);
            //    master.redirect_to(location, from_where);
            //}
            //else
            //{
            //    _Main master = (_Main)(page.Master);
            //    master.redirect_to(location, from_where);
            //}
        }
        else
        {
            HttpContext.Current.Response.Redirect(location);
        }
    }

    public static string NextPage(string navOption = "default")
    {
        //--- reads the next page to navigate to from the custom config file; each page can have multiple
        //--- targets denoted as target_[scenario]; this function will default to target_default
        return ReadCustomConfigValue(string.Concat("pages/", GetCurrentPage()), string.Concat("target_", navOption));
    }

    public static string NextPageByPage(string CurPage, string NavOption = "default")
    {
        //--- reads the next page to navigate to from the custom config file; each page can have multiple
        //--- targets denoted as target_[scenario]; this function will default to target_default
        return ReadCustomConfigValue(string.Concat("pages/", CurPage.ToLower()), string.Concat("target_", NavOption));
    }

    public static string GetCurrentPage()
    {
        //--- returns lowercase page name (ex: selfenroll.aspx), primarily for lookups in the custom config file
        // --- added the mobile section as the mobile pages are prefixed with "m" so when using WebTrends we can identify the page
        return HttpContext.Current.Request.Path.Contains("/mobile/") ? HttpContext.Current.Request.Path.Substring(HttpContext.Current.Request.Path.LastIndexOf("/") + 1).ToLower().Replace("m", "") : HttpContext.Current.Request.Path.Substring(HttpContext.Current.Request.Path.LastIndexOf("/") + 1).ToLower();
    }

    public static XmlNode GetNavSettingsConfigNode(string Section)
    {
        string ConfigFilePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["NavSettingsPath"]);

        //--- returns the links XmlNode from the custom navigation config file
        XmlDocument xmlDoc = default(XmlDocument);
        XmlNode node = default(XmlNode);

        xmlDoc = new XmlDocument();
        xmlDoc.Load(ConfigFilePath);
        node = xmlDoc.SelectSingleNode(string.Concat("//settings/", Section));

        return node;
    }

    public static XDocument GetNavSettingsConfig()
    {
        var configFilePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["NavSettingsPath"]);
        return XDocument.Load(configFilePath);
    }


    #endregion

    #region WebRegistration.config access

    public static bool IsClientFacingEnv()
    {
        //--- determines whether current instance of the site is in a client-facing environment
        //TODO: determine whether http://mp@www.PerfCentralURL.com incorrectly returns False

        string env = HttpContext.Current.Request.Url.Host.ToLower().Split(new[] { '.' })[0];
        string icf = ReadCustomConfigValue("url/" + env, "isclientfacing");
        if (icf == string.Empty)
        {
            return false;
        }
        return string.Compare(icf, "true", true) == 0;
    }

    public static string getRole()
    {
        // MS 6|9|14
        // there is a possibility that getHost() may return an empty string Ex. user does not type in www in the site url
        // in this case getRole() would return nothing. To avoid that we will check what getHost() is ans if it is Empty we will return Prod environment.
        // Because each app runs under app users account we are safe that the account will not have permissions to access Prod if we try calling in from lower environments
        if (IsNothingNullOrEmpty(getHost()))
        {
            // return prod
            return ReadCustomConfigValue("url/" + "www", "role");
        }

        return IsNothingNullOrEmpty(ReadCustomConfigValue("url/" + getHost(), "role")) ? ReadCustomConfigValue("url/" + "www", "role") : ReadCustomConfigValue("url/" + getHost(), "role");
    }

    public static bool isClientFacing()
    {
        return ReadCustomConfigValue("url/" + getHost(), "isclientfacing").toBool();
    }

    public static object GetCurPageConfigValue(string key, string NotFound = "")
    {
        return ReadCustomConfigValue("pages/" + GetCurrentPage(), key, NotFound);
    }

    public static bool ShouldReadWebRegSettings(string CurrentPage, bool default_result = true)
    {
        return Utilities.ReadCustomConfigValue("pages/" + CurrentPage + ".aspx", "option_readwebregsettings", default_result ? "true" : "false").toBool(default_result);
    }

    public static string GetCustomConfigPath()
    {
        return ConfigurationManager.AppSettings["CustomSettingsPath"].toStringOr("configuration/WebRegistration.config");
    }
    public static string GetCustomFieldsConfig()
    {
        return ConfigurationManager.AppSettings["CustomSettingsPath"].toStringOr("configuration/CustomFields.config");
    }
    public static string GetMappedCustomConfigPath()
    {
        return Utilities.MapPath(GetCustomConfigPath());
    }

    public static string ReadCustomConfigValue(string Section, string Key = "value", string NotFound = "")
    {
        //--- returns the specified value from the specified section of the custom config file
        Hashtable ht;

        string filepath = GetMappedCustomConfigPath();

        if (filepath.Contains("mobile"))
        {
            filepath = filepath.Replace("\\mobile\\", "\\");
        }

        Key = Key.ToLower();
        ht = GetCustomConfig(filepath, Section);

        if (ht == null || ht[Key] == null || ht[Key].ToString() == "")
        {
            return NotFound;
        }

        return ht[Key].ToString();
    }

    private static Hashtable GetCustomConfig(string ConfigFilePath, string Section, bool UseDictionary = false)
    {
        //--- parses the custom config XML file for the specified section and returns it as a hashtable if found
        //--- a dictionary can be used for name/value pairs where key may have spaces, etc.
        XmlDocument xmlDoc = new XmlDocument();
        SingleTagSectionHandler stHandler;
        DictionarySectionHandler dcHandler;
        XmlNode node;

        xmlDoc.Load(ConfigFilePath);
        node = xmlDoc.SelectSingleNode(string.Concat("//settings/", Section));

        if (node == null)
        {
            return null;
        }
        if (UseDictionary)
        {
            dcHandler = new DictionarySectionHandler();
            return (Hashtable)dcHandler.Create(null, null, node);
        }

        stHandler = new SingleTagSectionHandler();

        return (Hashtable)stHandler.Create(null, null, node);
    }

    /// <summary>
    /// Get program number from config or URL
    /// </summary>
    /// <returns></returns>
    

    public static Hashtable ReadCustomConfig(string Section)
    {
        //--- returns a hashtable of values for the specified section of the custom config file
        return GetCustomConfig(GetMappedCustomConfigPath(), Section);
    }

    public static Hashtable ReadCustomConfigNameValue(string Section)
    {
        //--- returns a hashtable of values for the specified section of the custom config file
        return GetCustomConfig(GetMappedCustomConfigPath(), Section, true);
    }

    public static DateTime publish_date
    {
        get
        {
            if (HttpContext.Current.Cache["pc_system_publish_date"] != null)
            {
                return (DateTime)HttpContext.Current.Cache["pc_system_publish_date"];
            }

            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);

            HttpContext.Current.Cache.Insert("pc_system_publish_date", dt, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0));

            return dt;
        }
    }

    #endregion

    #region Web.config access

    public static string getHost()
    {
        string host = HttpContext.Current.Request.Url.Host.ToLower();

        if (host.StartsWith("localhost") || host.StartsWith("local"))
        {
            return "dev";
        }
        else if (host.StartsWith("dev"))
        {
            return "dev";
        }
        else if (host.StartsWith("test"))
        {
            return "test";
        }
        else if (host.StartsWith("qa"))
        {
            return "qa";
        }
        else if (host.StartsWith("uat"))
        {
            return "uat";
        }
        else if (host.StartsWith("eisdev"))
        {
            return "eisdev";
        }
        else if (host.StartsWith("www"))
        {
            return "www";
        }
        else
        {
            return "www";
        }

    }

    public static string getEnvironment(string role = "")
    {
        if (string.IsNullOrEmpty(role))
        {
            role = getRole(); // defined in WebRegistration.config
        }

        string env = role;

        if (role.ToLower() == "local" || role.ToLower() == "localhost")
        {
            env = "CDT";
        }
        // MS 5.30.14 added new environments that will be part of the SQL 2012 rollout
        else if (role.ToLower() == "qa")
        {
            env = "QA";
        }
        else if (role.ToLower() == "eisdev")
        {
            env = "EISDEV";
        }
        else if (role.ToLower() == "dev" || role.ToLower() == "dev8")
        {
            env = "DEV";
        }
        else if (role.ToLower() == "uat")
        {
            env = "UAT";
        }
        // END
        else if (role.ToLower() == "develop" || role.ToLower() == "development" || role.ToLower() == "eisdev")
        {
            env = "IDI";
        }
        else if (role.ToLower() == "www")
        {
            env = "PR";
        }
        else if (role.ToLower() == "test")
        {
            env = "CA";
        }
        else if (role.ToLower() == "eislt")
        {
            env = "LT";
        }
        else
        {
            if (IsNothingNullOrEmpty(env))
            {
                env = "PR";
            }
        }

        return env;
    }

    private static string getRoleSettings()
    {
        string retval = "";

        NameValueCollection nvc = (NameValueCollection)ConfigurationManager.GetSection(string.Concat("Roles/", getRole()));
        if (nvc != null)
        {
            foreach (string key in nvc.AllKeys)
            {
                retval += "<tr><td style=\"text-align:left;\">" + key + "</td><td style=\"text-align:left;\">";
                if (key.ToLower().Contains("password"))
                {
                    retval += "*******************";
                }
                else
                {
                    retval += nvc[key];
                }
                retval += "</td></tr>";
            }
        }
        return retval;
    }

    public static string GetConfigValue(string Key, string default_val = "")
    {
        //--- looks up a value for the specified Key in the <Roles> section of the web.config based upon
        //--- the Role configured in the appSettings
        string role = getRole();
        string env = getEnvironment(role);

        string setting = ConfigurationManager.AppSettings[env.ToUpper() + "_" + Key].toStringOr();

        if (string.IsNullOrWhiteSpace(setting))
        {
            setting = ConfigurationManager.AppSettings[Key].toStringOr();
        }

        if (string.IsNullOrWhiteSpace(setting))
        {
            setting = default_val;
        }

        return ITA_Configuration.ReplaceConfigValue(setting, role, env);
    }

    private static string GetConnectionString(string DataBase, bool decrypt = true)
    {
        string role = getRole();
        string environment = getEnvironment(role);
        string connection = ConfigurationManager.ConnectionStrings[environment.ToUpper() + "_" + DataBase].toStringOr();

        if (string.IsNullOrWhiteSpace(connection))
        {
            connection = ConfigurationManager.ConnectionStrings[DataBase].toStringOr();
        }

        connection = ITA_Configuration.ReplaceConfigValue(connection, role, environment);

        if (!decrypt) return connection;

        return PCentralConfiguration.decrypt_connection_string(connection);
    }

    public static string GetEnvironmentConnection()
    {
        return Utilities.GetConnectionString("PerformanceCentral");
    }

    public static string GetEnvironmentConnectionITAEnterprise()
    {
        return Utilities.GetConnectionString("ITAEnterprise");
    }

    public static string GetEnvironmentConnectionWebRegCustom()
    {
        return Utilities.GetConnectionString("WebRegCustom");
    }

    public static string GetEnvironmentConnectionWebRegAdmin()
    {
        return Utilities.GetConnectionString("WebRegAdmin");
    }

    #endregion

    #region CustomMessages.config access

    /*
    public static Hashtable ReadMessageConfigValues(string Section)
    {
        string mspath = ConfigurationManager.AppSettings["MessageSettingsPath"].toStringOr("configuration/CustomMessages.config");

        //--- returns a hashtable of values for the specified section of the custom config file
        return GetCustomConfig(HttpContext.Current.Server.MapPath(mspath), Section);
    }
    */

    public static string ReadMessageConfigValue(string Section, string Key = "value", string NotFound = "")
    {
        string mspath = ConfigurationManager.AppSettings["MessageSettingsPath"].toStringOr("configuration/CustomMessages.config");

        //--- returns the specified value from the specified section of the custom config file
        Hashtable ht = GetCustomConfig(HttpContext.Current.Server.MapPath(mspath), Section);

        if (ht == null || ht[Key] == null || string.IsNullOrEmpty(ht[Key].ToString()))
        {
            return NotFound;
        }
        else
        {
            return ht[Key].ToString();
        }
    }

    #endregion

    #region Page-Level Business Errors

    public static string DisplayAllErrorsAndWarningsbr(PCentralValidationResults oRequiredFields)
    {
        StringBuilder warnings = new StringBuilder();

        for (int i = 0; i < oRequiredFields.errors.Count; i++)
        {
            if (oRequiredFields.errors[i] == "Error 623")
            {
                oRequiredFields.errors[i] = Utilities.ReadMessageConfigValue("rules/Accommodation_NoAccomToAccom");
            }
        }

        warnings.Append(oRequiredFields.get_all_errors_br(false));

        foreach (string element in oRequiredFields.warnings)
        {
            warnings.Append(element);
            warnings.Append("<br />");
        }

        return Regex.Replace(warnings.ToString(), "\\s*\\(BR\\d{1,4}\\)", string.Empty);
    }

    public static string DisplayAllErrorsAndWarningscomma(PCentralValidationResults oRequiredFields)
    {
        StringBuilder warnings = new StringBuilder();

        for (int i = 0; i < oRequiredFields.errors.Count; i++)
        {
            if (oRequiredFields.errors[i] == "Error 623")
            {
                oRequiredFields.errors[i] = Utilities.ReadMessageConfigValue("rules/Accommodation_NoAccomToAccom");
            }
        }

        warnings.Append(oRequiredFields.get_all_errors_comma(false));

        if (warnings.ToString().Length > 0)
            warnings.Append("<br />");

        foreach (string element in oRequiredFields.warnings)
        {

            warnings.Append(element);
            warnings.Append(", ");
        }

        if (warnings.ToString().EndsWith(", "))
            warnings = warnings.Remove(warnings.Length - 2, 2);

        return Regex.Replace(warnings.ToString(), "\\s*\\(BR\\d{1,4}\\)", string.Empty);
    }

    public static void CheckRequired(string value, string errormessage, ref PCentralValidationResults oRequiredFields)
    {
        if (value.Trim().Length == 0)
        {
            oRequiredFields.add_required_field_error(errormessage, false);
        }
    }

    public static void CheckRequired(string value, string errormessage, ref PCentralValidationResults oRequiredFields, string append)
    {
        if (value.Trim().Length == 0)
        {
            oRequiredFields.add_error("Required field " + errormessage + " is missing" + append, false);
        }
    }

    public static void CheckRequired(int value, string errormessage, ref PCentralValidationResults oRequiredFields)
    {
        if (value < 1)
        {
            oRequiredFields.add_required_field_error(errormessage, false);
        }
    }

    public static void CheckRequired(int value, string errormessage, ref PCentralValidationResults oRequiredFields, string append)
    {
        if (value < 1)
        {
            oRequiredFields.add_error("Required field " + errormessage + " is missing" + append, false);
        }
    }

    // MS new codeline
    public static void DisplayMessages(Label errorLabel, PCentralValidationResults oValidationResults, bool saveErrorMessage = false)
    {
        errorLabel.CssClass = "warning";
        errorLabel.Text = Utilities.DisplayAllErrorsAndWarningsbr(oValidationResults);
    }

    public static void DisplayMessagesComma(Label errorLabel, PCentralValidationResults oValidationResults, bool saveErrorMessage = false)
    {
        errorLabel.CssClass = "warning";
        errorLabel.Text = Utilities.DisplayAllErrorsAndWarningscomma(oValidationResults);
    }
    public static void ResetAlternateParticipantEvent()
    {

        PCentralParty varParty = PCentralParty.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        foreach (PCentralParticipant participant in varParty.Participants)
        {
            using (DB mssql = new DB(Utilities.GetEnvironmentConnectionWebRegCustom()))
            {
                mssql.Get_DataSet("[dbo].[spResetAlternateparticipantevent]", DB.MakeParam("ParticipantID", participant.ParticipantID));

            }
        }
    }

    #endregion

    #region Custom Fields

    /// <summary>
    /// Get all the custom field values for a given participant
    /// </summary>
    /// <param name="participantID"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetAllCustomFieldValues(int participantID)
    {
        return PCentralProgramCustomField.GetCustomFieldValuesByParticipantID(Utilities.GetEnvironmentConnection(), participantID);
    }

    /// <summary>
    /// Use if you have a custom logic that needs to remove or switch if CF is required
    /// </summary>
    /// <param name="cfnames">semi-colon delimited list of custom fields</param>
    /// <param name="cfstatus"></param>
    /// <param name="removeCF">semi-colon delimited list of custom fields that should be removed</param>
    /// <param name="ChangeStatus"></param>
    public static void GetCustomfieldNameandStatus(string cfnames, string cfstatus, string removeCF, SortedList<string, string> ChangeStatus = null)
    {
        ArrayList arrayRequiredItems = new ArrayList();
        SortedList<string, string> sortedList = null;

        // Both CustomFieldsRequired and CustomFields are string data type. The reason is data base schema.
        // Each string is delimited by (;)
        // I force to have a exactly 1 item in CustomFieldsRequired for each selected item in CustomFields

        //required items for each CF in PreProfileCustomFields collection
        foreach (string s in cfstatus.Split(new[] { ';' }))
        {
            arrayRequiredItems.Add(s);
        }

        int i = 0; // set for array counter

        if (!Utilities.IsNothingNullOrEmpty(cfnames))
        {
            sortedList = new SortedList<string, string>();

            foreach (string cf in cfnames.Split(new[] { ';' }))
            {
                sortedList.Add(cf, arrayRequiredItems[i].ToString());
                i++; // increment counter
            }

            if (!Utilities.IsNothingNullOrEmpty(removeCF))
            {
                foreach (string s in removeCF.Split(new[] { ';' }))
                {
                    if (sortedList.ContainsKey(s))
                    {
                        sortedList.Remove(s);
                    }
                }
            }

            //check if any values needed to change
            if (!Utilities.IsNothingNullOrEmpty(ChangeStatus))
            {
                foreach (KeyValuePair<string, string> item in ChangeStatus)
                {
                    if (sortedList.ContainsKey(item.Key))
                    {
                        sortedList[item.Key] = item.Value;
                    }
                }
            }

            StringBuilder newcfname = new StringBuilder();
            StringBuilder newcfstatus = new StringBuilder();

            foreach (KeyValuePair<string, string> item in sortedList)
            {
                newcfname.Append(item.Key);
                newcfname.Append(";");
                newcfstatus.Append(item.Value);
                newcfstatus.Append(";");
            }

            if (newcfname.Length > 0)
            {
                newcfname.Remove((newcfname.Length - 1), 1); // remove the last
                cfnames = newcfname.ToString(); // update collection
            }
            else if (sortedList.Count == 0) //items were removed
            {
                cfnames = string.Empty;
            }

            if (newcfstatus.Length > 0)
            {
                newcfstatus.Remove((newcfstatus.Length - 1), 1); // remove the last
                cfstatus = newcfstatus.ToString(); // update collection
            }
            else if (sortedList.Count == 0) //items were removed
            {
                cfstatus = string.Empty;
            }
        }
    }

    #endregion

    #region Exception Handling

    private static string getAppSettings()
    {
        string retval = "";

        // Get the appSettings.
        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        // Get the collection enumerator.
        IEnumerator appSettingsEnum = appSettings.Keys.GetEnumerator();

        // Loop through the collection and
        // display the appSettings key, value pairs.
        int i = 0;
        while (appSettingsEnum.MoveNext())
        {
            string key = appSettings.Keys[i];
            retval += "<tr><td style=\"text-align:left;\">" + key + "</td><td style=\"text-align:left;\">";
            if (key.ToLower().Contains("password"))
            {
                retval += "*******************";
            }
            else
            {
                retval += appSettings[key];
            }
            retval += "</td></tr>";
            i++;
        }

        return retval;
    }

    private static string getAppConfig()
    {
        return "<tr><td colspan='2' style=\"text-align:left;\">" + Utilities.LoadFileContent("application.config").ToString() + "</td></tr>";
    }

    /*
    private static string getSessionVars()
    {
        string retval = "";

        for (int iKey = 0; iKey < HttpContext.Current.Session.Keys.Count; iKey++)
        {
            retval += "<tr><td>" + HttpContext.Current.Session.Keys[iKey].ToString() + "</td><td>";
            retval += HttpContext.Current.Session[iKey].ToString();
            retval += "</td></tr>";
        }

        return retval;
    }
    */

    #endregion

    #region Miscellaneous Functions
    public static string LoadCMSContentForDisplay(string pageName, string uniqueID)
    {
        DataSet dsContent = null;

        using (DB mssql = new DB(Utilities.GetEnvironmentConnectionWebRegAdmin()))
        {
            List<SqlParameter> param_list = new List<SqlParameter>();

            param_list.Add(DB.MakeParam("ProgramID", ContextManager.ProgramID));
            param_list.Add(DB.MakeParam("PageName", pageName));
            param_list.Add(DB.MakeParam("UniqueID", uniqueID));
            param_list.Add(DB.MakeParam("ParticipantID", ContextManager.ParticipantID));

            dsContent = mssql.Get_DataSet("dbo.spCMSContentForDisplaySelect", param_list);
        }

        if (dsContent.Tables[0].Rows.Count > 0)
        {
            return dsContent.Tables[0].Rows[0][0].ToString();
        }
        return string.Empty;

    }

    internal static DataTable RemoveUnusedAirlines(DataTable dtAirlineList)
    {

        //var dtAirlineList = PCentralCodeLookup.airlines(Utilities.GetEnvironmentConnection()).GetDataTable();
        var rowsToDelete = new List<DataRow>();

        //list of  airlines no longer in business
        string[] airlinesToRemove = { "continental", "midwest express", "northwest", "usairways" };


        rowsToDelete = dtAirlineList.Rows.Cast<DataRow>().Where(row => airlinesToRemove.Contains(row["DisplayValue"].ToString().ToLower().Trim())).ToList();
        rowsToDelete.ForEach(x => dtAirlineList.Rows.Remove(x));

        dtAirlineList.AcceptChanges();
        return dtAirlineList;
    }

    public static int YearsBetweenDates(DateTime StartDate, DateTime EndDate)
    {
        DateTime today = EndDate;
        int age = today.Year - StartDate.Year;
        if (StartDate > today.AddYears(-age))
            return age - 1;
        return age;
    }
    public static bool isMobileBrowser()
    {
        //GETS THE CURRENT USER CONTEXT
        HttpContext context = HttpContext.Current;
        //FIRST TRY BUILT IN ASP.NET CHECK
        if (context.Request.Browser.IsMobileDevice)
        {
            return true;
        }
        //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
        if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
        {
            return true;
        }
        //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
        if (context.Request.ServerVariables["HTTP_ACCEPT"] != null && context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
        {
            return true;
        }
        //AND FINALLY CHECK THE HTTP_USER_AGENT
        //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING

        if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
        {
            //Create a list of all mobile types
            string[] mobiles = {
			"midp",
			"j2me",
			"avant",
			"docomo",
			"novarra",
			"palmos",
			"palmsource",
			"240x320",
			"opwv",
			"chtml",
			"pda",
			"windows ce",
			"mmp/",
			"blackberry",
			"mib/",
			"symbian",
			"wireless",
			"nokia",
			"hand",
			"mobi",
			"phone",
			"cdm",
			"up.b",
			"audio",
			"SIE-",
			"SEC-",
			"samsung",
			"HTC",
			"mot-",
			"mitsu",
			"sagem",
			"sony",
			"alcatel",
			"lg",
			"eric",
			"vx",
			"NEC",
			"philips",
			"mmm",
			"xx",
			"panasonic",
			"sharp",
			"wap",
			"sch",
			"rover",
			"pocket",
			"benq",
			"java",
			"pt",
			"pg",
			"vox",
			"amoi",
			"bird",
			"compal",
			"kg",
			"voda",
			"sany",
			"kdd",
			"dbt",
			"sendo",
			"sgh",
			"gradi",
			"jb",
			"dddi",
			"moto",
			"iphone",
			"Android"
		};
            //New () {"midp", "j2me", "avant", "docomo", "novarra", "palmos", _   "palmsource", "240x320", "opwv", "chtml", "pda", "windows ce", _   "mmp/", "blackberry", "mib/", "symbian", "wireless", "nokia", _   "hand", "mobi", "phone", "cdm", "up.b", "audio", _   "SIE-", "SEC-", "samsung", "HTC", "mot-", "mitsu", _   "sagem", "sony", "alcatel", "lg", "eric", "vx", _   "NEC", "philips", "mmm", "xx", "panasonic", "sharp", _   "wap", "sch", "rover", "pocket", "benq", "java", _   "pt", "pg", "vox", "amoi", "bird", "compal", _   "kg", "voda", "sany", "kdd", "dbt", "sendo", _   "sgh", "gradi", "jb", "dddi", "moto", "iphone"}
            //Loop through each item in the list created above
            //and check if the header contains that text
            foreach (string s in mobiles)
            {
                if (context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains(s.ToLower()) && !context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains("ipad"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static object NZ(object Value, object ValueIfEmpty)
    {
        //--- returns ValueIfEmpty if Value is empty, otherwise Value
        if (Utilities.IsNothingNullOrEmpty(Value))
        {
            return ValueIfEmpty;
        }
        else
        {
            return Value;
        }
    }

    public static DateTime GetDate(object Value, System.DateTime ValueIfNotDate)
    {
        if (Utilities.IsNothingNullOrEmpty(Value))
        {
            return ValueIfNotDate;
        }
        else if (Value.isDateTime())
        {
            return Value.toDateTime();
        }
        else
        {
            return ValueIfNotDate;
        }
    }

    /*
    public static bool IsDate(object Expression)
    {
        DateTime dateValue;

        return DateTime.TryParseExact(Convert.ToString(Expression), new string[] { "d", "g" }, System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out dateValue) && Convert.ToDateTime(Expression) >= Convert.ToDateTime("1/1/1800");
    }
    */

    public static string ReadPartyRegistrationStatus()
    {
        PCentralParty varParty = PCentralParty.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.PartyID);

        return varParty.RegistrationStatus.ToString();
    }

    public static void LimitTextAreaLength(ref TextBox txt, Int32 maxCharacters)
    {
        txt.Attributes.Add("onkeydown", string.Concat("javascript: limitText(document.getElementById('", txt.ClientID, "'), ", maxCharacters.ToString(), ");"));
        txt.Attributes.Add("onkeyup", string.Concat("javascript: limitText(document.getElementById('", txt.ClientID, "'), ", maxCharacters.ToString(), ");"));
    }

    public static string MapPath(string path)
    {
        path = path.Trim();

        if (!Regex.IsMatch(path, "^[a-z|A-Z]:.*"))
        {
            if (path.StartsWith("/"))
            {
                path = "~" + path;
            }
            else if (!path.StartsWith("~"))
            {
                path = "~/" + path;
            }
        }

        return HttpContext.Current.Server.MapPath(path);
    }

    public static StringBuilder LoadFileContent(string fileName)
    {
        System.IO.StreamReader sr = null;
        StringBuilder sbBody = new StringBuilder();
        string sFile = Utilities.MapPath(fileName);

        if (System.IO.File.Exists(sFile))
        {
            sr = System.IO.File.OpenText(sFile);
            sbBody.Append(sr.ReadToEnd());
            sr.Close();
        }
        else
        {
            //*
           
            Exception ex = new Exception("Missing EMail Template '" + sFile + "' was not found for FlyITA program.");

            ex.AddData("Location", "Utilities:LoadFileContent");
            ex.AddData("fileName", fileName);

            throw ex;
            //*/
        }

        return sbBody;
    }

    public static void LoadList(ListControl ctl, ref DataTable source, string ValueField = "CodeLookupId", string TextField = "DisplayValue", string DefaultText = null, bool Sort = true, string Criteria = "", bool CheckNoDefault = false, string sortOptionsby = "")
    {
        //--- fills the referenced ListControl with the referenced table, optionally sorting the list
        //--- and inserting a default option at the beginning of the list, (ex: "Select one...")
        if (Sort)
        {
            if (source.DefaultView.Table.Columns.Contains("defaultFLG"))
            {
                source.DefaultView.Sort = string.Concat("defaultFLG DESC, ", TextField, " ASC");
            }
            else if (sortOptionsby.isNotEmpty())
            {
                source.DefaultView.Sort = sortOptionsby;
            }
            else
            {
                source.DefaultView.Sort = string.Concat(TextField, " ASC");
            }
        }

        source.DefaultView.RowFilter = Criteria;
        ctl.DataSource = source;
        ctl.DataValueField = ValueField;
        ctl.DataTextField = TextField;
        ctl.DataBind();

        if (!CheckNoDefault)
        {
            if (source.DefaultView.Table.Columns.Contains("defaultFLG"))
            {
                source.DefaultView.RowFilter = string.Concat("defaultFLG = 1", "", "", Criteria);
                if (source.DefaultView.Count > 0)
                {
                    ctl.SelectedValue = source.DefaultView[0].Row[ValueField].ToString();
                }
                else
                {
                    if (DefaultText != null)
                    {
                        ctl.Items.Insert(0, new ListItem(DefaultText, ""));
                    }
                }
            }
            else
            {
                if (DefaultText != null)
                {
                    ctl.Items.Insert(0, new ListItem(DefaultText, ""));
                }
            }
        }
        else if (DefaultText != null)
        {
            ctl.Items.Insert(0, new ListItem(DefaultText, ""));
        }
    }

    public static bool IsNothingNullOrEmpty(object Value)
    {
        //--- for shorthand use with objects that may be null references or otherwise empty
        if (Value == null) return true;
        if (object.ReferenceEquals(Value, DBNull.Value)) return true;
        if (Value.ToString().Trim().Length == 0) return true;
        return false;
    }
    public static bool IsNothingNullOrEmptyorZero(object Value)
    {
        //--- for shorthand use with objects that may be null references or otherwise empty
        if (Value == null) return true;
        if (Value.ToString() == "0") return true;
        if (object.ReferenceEquals(Value, DBNull.Value)) return true;
        if (Value.ToString().Trim().Length == 0) return true;
        return false;
    }

    public static bool IsValidDateTime(object Value)
    {
        //--- for shorthand use with objects that may be null references or otherwise empty
        DateTime dt = Value.toDateTime(DateTime.MinValue);
        return dt > new DateTime(1753, 1, 1);
    }

    /*
    public static bool IsNothingNullOrEmptyZero(object Value)
    {
        //--- for shorthand use with objects that may be null references or otherwise empty
        if (Convert.ToInt32(Value) == 0) return true;
        if (Value == null) return true;
        if (object.ReferenceEquals(Value, DBNull.Value)) return true;
        if (Value.ToString().Trim().Length == 0) return true;
        return false;
    }
    */

    /*
    public static object IsNullIntegerThenZero(object Value)
    {
        if (Value == null) return 0;
        return Value;
    }
    */

    /*
    public static string JoinStringArrayList(string separator, ref ArrayList values)
    {
        //--- similar to String.Join but will skip over array items with no contents to avoid extra separators
        StringBuilder sb;

        sb = new StringBuilder();

        for (int index = 0; index < values.Count; index++)
        {
            if (!IsNothingNullOrEmpty(values[index]))
            {
                sb.Append(values[index]);
                sb.Append(separator);
            }
        }

        if (sb.Length > 0)
        {
            return sb.ToString().Substring(0, sb.Length - separator.Length);
        }
        else
        {
            return string.Empty;
        }
    }
    */

    /*
    public static object IIf(bool Expression, object TruePart, object FalsePart)
    {
        object ReturnValue = Expression == true ? TruePart : FalsePart;

        return ReturnValue;
    }
    */

    /*
    public static string GetCodeLookupValue(DataSet ds, int CodeLookupID)
    {
        //--- retrieves DisplayValues from standard Code Lookups with CodeLookupID primary key

        //if (ds.Tables[0].PrimaryKey == null)
        //{
        ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns["CodeLookupID"] };
        //}

        return ds.Tables[0].Rows.Find(CodeLookupID)["DisplayValue"].ToString();
    }
    */

    public static void SetFocus(System.Web.UI.Control ctl)
    {
        //--- writes javascript to the end of the calling page to set focus to the referenced control;
        //--- .ClientID corrects for container name prefixes; setTimeout() avoids inconsistent IE bug
        ctl.Page.ClientScript.RegisterStartupScript(ctl.Page.GetType(), "PageFocus", string.Concat("setTimeout(", (char)34, "document.getElementById('", ctl.ClientID, "').focus();", (char)34, ", 1);"), true);
    }

    public static int GetProgramIDByProgramNbr(string ProgramNbr, Label ErrorLabel)
    {
        //--- returns the ProgramID for a given ProgramNumber, ex: 2D4G7L
        PCentralProgram program = PCentralLib.WebReg.PCentralWebRegFunctions.GetProgram(GetEnvironmentConnection(), ProgramNbr);

        return program.ProgramID;
    }

    /*
    public static void AddToCollection(ref SortedList<string, string> collection, string key, string value)
    {
        if (collection.ContainsKey(key) == false)
        {
            collection.Add(key, value);
        }
    }
    */

    //public static DataSet CreateAppsList()
    //{
    //    // bind from test file
    //    DataSet ds = new DataSet();
    //    ds.ReadXml(HttpContext.Current.Server.MapPath("~/WebRegResourceFiles/Grid.xml"));
    //    return ds;
    //}

    /*
    public static Boolean IsNumeric(string stringToTest)
    {
        int result;
        return int.TryParse(stringToTest, out result);
    }

    public static string ValidateInput(string value, string errormsg, ref StringBuilder list, string delimiter = ",")
    {
        if (IsNothingNullOrEmpty(value))
        {
            list.Append(delimiter);
            list.Append(errormsg);
        }
        return errormsg;
    }
    */

    public static bool IsDate(string date)
    {
        return date.isDateTime();
    }

    public static string formatDate(DateTime date, string format = "", string default_result = "")
    {
        if (string.IsNullOrWhiteSpace(format)) format = ContextManager.DateFormat;

        if (date.isEmptyDate())
        {
            return default_result;
        }
        else
        {
            return date.ToString(format);
        }
    }

    public static string formatDateString(string date, string format = "", string default_result = "")
    {
        if (string.IsNullOrWhiteSpace(format)) format = ContextManager.DateFormat;

        if (date.isEmptyDate())
        {
            return default_result;
        }
        else
        {
            return date.toDateTime().ToString(format);
        }
    }

    /// <summary>
    /// Returns the sort time string for a date if the time is not 12:00 AM - Else returns alternate
    /// </summary>
    /// <param name="date"></param>
    /// <param name="alternate">What to return for 12:00 AM (defaults to nothing)</param>
    /// <returns></returns>
    public static string timeIfNotMidnight(DateTime date, string alternate = "")
    {
        if (date != date.Date)
        {
            return date.ToShortTimeString();
        }

        return alternate;
    }

    public static string Formatlinereturn(string inputstring)
    {
        if (!IsNothingNullOrEmpty(inputstring))
        {
            return inputstring.Replace(Environment.NewLine, ", ");
        }
        return inputstring;
    }


    #endregion
}