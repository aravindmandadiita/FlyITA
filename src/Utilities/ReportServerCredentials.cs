using ITALib;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Net;
using System.Security.Principal;

[Serializable()]
public class ReportServerCredentials : IReportServerCredentials
{
    private string _username { get; set; }
    private string _userpassword { get; set; }
    private string _userdomain { get; set; }

    public void New()
    {
    }

    public void New(string userName, string userPassword, string userDomain)
    {
        _username = userName;
        _userpassword = userPassword;
        _userdomain = userDomain;
    }

    public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
    {
        // not use FormsCredentials unless you have implements a custom authentication.
        authCookie = null;
        user = password = authority = null;
        return false;
    }

    public WindowsIdentity ImpersonationUser
    {
        get
        {
            return null; // not use ImpersonationUser
        }
    }

    public ICredentials NetworkCredentials
    {
        get
        {
            try
            {
                string username = Utilities.GetConfigValue("SOAPRptUserName").toStringOr();
                string pwd = Utilities.GetConfigValue("SOAPRptPassword").toStringOr();
                string domain = Utilities.GetConfigValue("SOAPRptDomain").toStringOr();

                // use NetworkCredentials
                if (!string.IsNullOrEmpty(_username))
                {
                    return new NetworkCredential(_username, _userpassword, _userdomain);
                }
                else
                {
                    Configuration myConfiguration = ConfigurationManager.OpenMachineConfiguration();

                    return new NetworkCredential(username, pwd, domain);
                }
            }
            catch // (Exception ex)
            {
                return null;
            }
        }
    }
}

