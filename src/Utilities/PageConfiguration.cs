using ITALib.DataAccess;
using System.Data;
using System.Web;
using ITALib;
using System;

public class PageConfiguration
{
    public bool PersonMatchingEnabled(string itaprogramnumber = "")
    {
        string connection = Utilities.GetEnvironmentConnectionWebRegAdmin();

        itaprogramnumber = GetITAProgNbr(itaprogramnumber);

        bool? item_value = DB.GetVar(connection, "SELECT ItemValue FROM WRAPageConfiguration WHERE ItemName = 'option_person_matching' AND PageName = 'selfenroll' AND ITAProgramNumber = {0}", itaprogramnumber).toBoolNullable();

        if (item_value == null)
        {
            item_value = Utilities.ReadCustomConfigValue("pages/selfenroll.aspx", "option_person_match", "False").toBool(false);
        }

        return item_value ?? false;
    }

    public DataSet ReadPageConfigSettings(string callingpage, string itaprogramnumber = "")
    {
        string program_num = GetITAProgNbr(itaprogramnumber);

        using (DB mssql = new DB(Utilities.GetEnvironmentConnectionWebRegAdmin()))
        {
            try
            {
                return mssql.Get_DataSet("spWRASelPageConfiguration", program_num, callingpage);
            }
            catch (Exception ex)
            {
                ex.AddData("Location", "PageConfiguration:ReadPageConfigSettings");
                ex.AddData("Connection", ITALogging.LoggingReport.HideConnectionStringPassword(mssql.connection_string));
                ex.AddData("ITAProgramNumber", itaprogramnumber);
                ex.AddData("CallingPage", callingpage);

                throw;
            }
        }
    }

    private string GetITAProgNbr(string itaprogramnumber = "")
    {
        string ITAProgNbr = Utilities.ReadCustomConfigValue("ITAProgNbr", "value", string.Empty);

        if (string.IsNullOrWhiteSpace(ITAProgNbr))
        {
            ITAProgNbr = HttpContext.Current.Request.Path.Split(new[] { '/' })[1];

            if (string.IsNullOrWhiteSpace(ITAProgNbr) && !string.IsNullOrWhiteSpace(itaprogramnumber))
            {
                ITAProgNbr = itaprogramnumber;
            }
        }

        return ITAProgNbr;
    }
}
