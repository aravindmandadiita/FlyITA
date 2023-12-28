using System.Web.UI.HtmlControls;
using ITALib;
using System;
using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Xml;
using PCentralLib;
using PCentralLib.programs;

namespace FlyITA
{
    public partial class _Main : System.Web.UI.MasterPage
    {
        //private Label lbl = null;

        public void redirect_to(string to, string from)
        {
            this.hlnkRedirect.Text = to;
            this.hlnkRedirect.NavigateUrl = to + "nbsp;From:nbsp;" + from;
            this.hlnkRedirect.Visible = true;
            this.Main.Visible = false;
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            string strCurrentPage = Utilities.GetCurrentPage();
            Hashtable ht = null;
            string jsOptDisableForwardScript = null;
            string jsOptTimeoutWarningSeconds = null;
            string jsOptTimeoutSeconds = null;
            string jsOptScrollToTopOfWindow = null;
            bool blnDisplaySSLSeal = false;

            try
            {
                lblVersion.Text = Utilities.publish_date.ToString();

                ht = Utilities.ReadCustomConfig(string.Concat("pages/", strCurrentPage.ToLower()));

                if (ht != null)
                {
                    // first check if the user is on mobile device and tries to access registration pages,
                    // if so, redirect them to mobile message
                    if (!Utilities.IsNothingNullOrEmpty(ht["requires_authentication"]) && !Utilities.IsNothingNullOrEmpty(ContextManager.MobileType))
                    {
                        // is on mobile
                        if (ht["requires_authentication"].toBool())
                        {
                            Utilities.Redirect("./mobile/mRegistration.aspx");
                        }
                    }

                    // check if the page needs to run under SSL, this is mainly used for running i-frames, such us weather widgets that run without SSL and throw warning messages
                    // on a pages that runs under SSL. to avoid that, just run the page that has the widget without SSL
                    /*
                    if (ht.ContainsKey("rununderssl") && ht["rununderssl"].toBool() == false)
                    {
                        /// Run under http!
                        if (Request.IsSecureConnection)
                        {
                            Utilities.Redirect(Request.Url.AbsoluteUri.Replace("https://", "http://"));
                        }
                    }
                    else if (!(Request.IsSecureConnection || Request.IsLocal)) /// Run under https unless we're local!
                    {
                        Utilities.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
                    }
                    //*/
                }
                else if (!(Request.IsLocal || Request.IsSecureConnection))
                {
                    //Utilities.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("Location", "Master: HTTP Redirect");

                throw;
            }

            blnDisplaySSLSeal = Convert.ToBoolean(Utilities.ReadCustomConfigValue("DisplaySSLSeal"));
            jsOptScrollToTopOfWindow = Utilities.ReadCustomConfigValue("ScrollToWindowTop");
            jsOptDisableForwardScript = Utilities.ReadCustomConfigValue("DisableForwardScript");
            jsOptTimeoutWarningSeconds = (Utilities.ReadCustomConfigValue("TimeoutWarningSeconds"));
            jsOptTimeoutSeconds = (Utilities.ReadCustomConfigValue("TimeoutSeconds"));

            if (ht != null)
            {
                // the application is configured to secure this page
                if (ht["requires_authentication"] != null && string.Compare(ht["requires_authentication"].ToString(), "true", true) == 0)
                {
                    // this page requires the participant to be logged in
                    //Added to check if the user is CMS administrator
                    //if (ContextManager.ParticipantID == 0 && ContextManager.CMSUser == string.Empty)
                    //{
                    //    // no one is logged in
                    //    Utilities.Redirect(Utilities.ReadCustomConfigValue("MissingAuthenticationTarget", "value", "default.aspx"));
                    //}
                }

                if (string.Compare((Session["CurrentPage"] != null ? Session["CurrentPage"].ToString() : ""), strCurrentPage, true) == 0)
                {
                    // the user is on the current page, no need to check further
                }
                else
                {
                    if (ht["enforce_nav"] != null && string.Compare(ht["enforce_nav"].ToString(), "true", true) == 0)
                    {
                        // this page requires controlled navigation
                        if (string.Compare(Session["NextPage"].ToString(), strCurrentPage, true) == 0)
                        {
                            // the user has arrived at the "next page"
                            Session["NextPage"] = string.Empty;
                            Session["CurrentPage"] = strCurrentPage;
                        }
                        else
                        {
                            //check if this is CMS administrator
                            //if (ContextManager.CMSUser == string.Empty)
                            //{
                            //    // invalid navigation
                            //    if (Session["CurrentPage"] != null)
                            //    {
                            //        Utilities.Redirect(Session["CurrentPage"].ToString());
                            //    }
                            //    else
                            //    {
                            //        Utilities.Redirect(Utilities.ReadCustomConfigValue("InvalidNavigationTarget", "value", "default.aspx"));
                            //    }
                            //}
                        }
                    }
                }

                // navigation will display by default as per request from TS - Marek
                if (ht["display_nav"] != null && string.Compare(ht["display_nav"].ToString(), "false", true) == 0)
                {
                    this.mainmenu.Visible = false;
                    this.pnlNavLeft.Visible = false;
                    this.pnlNavFooter.Visible = false;
                }
                if ((ht["display_logo"] != null || ht["display_nav_menu"] != null))  // navigation will display the logo
                {
                    this.mainmenu.Visible = true;
                    foreach (Control ctl in this.mainmenu.Controls)
                    {
                        if (ctl.IsNull()) continue;
                        if (ctl.ID.IsNull()) continue;
                        if (ctl.ID.Contains("globallogo"))
                        {
                            ctl.Visible = false;
                            if (ht["display_logo"] != null)
                            {
                                ctl.Visible = Convert.ToBoolean(ht["display_logo"].ToString());
                            }
                        }
                        if (ctl.ID.Contains("globalmenu"))
                        {
                            ctl.Visible = false;
                            if (ht["display_nav_menu"] != null)
                            {
                                ctl.Visible = Convert.ToBoolean(ht["display_nav_menu"].ToString());
                            }
                        }
                    }
                }
                if (ht["display_reg_nav"] != null && string.Compare(ht["display_reg_nav"].ToString(), "true", true) == 0)
                {
                    this.pnlRegistrationNavigation.Visible = true;
                    LoadRegistrationNavigation();
                }

                if (ht["display_regfooter"] != null && string.Compare(ht["display_regfooter"].ToString(), "true", true) == 0)
                {
                    this.ucRegistrationFooter.Visible = true;
                }
                if (ht["display_mainfooter"] != null && string.Compare(ht["display_mainfooter"].ToString(), "false", false) == 0)
                {
                    this.mainfooter.Visible = false;
                }

                // page overrides for options
                if (ht["display_ssl_seal"] != null)
                {
                    blnDisplaySSLSeal = Convert.ToBoolean(ht["display_ssl_seal"]);
                }

                if (ht["timeout_warning_seconds"] != null && ht["timeout_warning_seconds"].isNumeric())
                {
                    jsOptTimeoutWarningSeconds = ht["timeout_warning_seconds"].ToString();
                }

                if (ht["timeout_seconds"] != null && ht["timeout_seconds"].isNumeric())
                {
                    jsOptTimeoutSeconds = ht["timeout_seconds"].ToString();
                }

                if (ht["disable_forward_script"] != null && string.Compare(ht["disable_forward_script"].ToString(), "true", true) == 0)
                {
                    jsOptDisableForwardScript = "true";

                }

                if (ht["scrollto_top_window"] != null)
                {
                    blnDisplaySSLSeal = Convert.ToBoolean(ht["display_ssl_seal"]);
                }

                //check if we need to assign registration styles
                if (ht["registrationstyle"] != null)
                {
                    this.registrationstyles.Href = ht["registrationstyle"].ToString();
                }
            }

            //if (pnlNavLeft.Visible || pnlNavFooter.Visible)
            //{
            //    BuildNav();
            //}

            this.ucSSLSealHeader.Visible = blnDisplaySSLSeal;
            this.pnlSSLSeal.Visible = blnDisplaySSLSeal;

            //optTimeoutWarningSeconds.Value = jsOptTimeoutWarningSeconds;
            //optTimeoutSeconds.Value = jsOptTimeoutSeconds;
            //optDisableForwardNav.Value = jsOptDisableForwardScript;
            //optScrollToTopOfWindow.Value = jsOptScrollToTopOfWindow;

        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //PCentralProgram program = PCentralProgram.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID.ToString());
            //this.lblClientName.Text = program.TravelHeadquartersName;
            //this.lblProgramTollfreeNbr.Text = program.ProgramTollFreeNbr;
            //this.lblMailto.Text = program.FromEmailAddress;
            //this.mailtoEmail.HRef = "mailto:" + program.FromEmailAddress;
        }

        protected void BuildNav()
        {
            System.Xml.XmlNode xmlLinks = default(System.Xml.XmlNode);
            SingleTagSectionHandler handler = default(SingleTagSectionHandler);
            Hashtable ht = null;
            string strLeftStart = "";
            string strLeftEnd = "";
            string strLeftSeparator = "";
            string strFooterStart = "";
            string strFooterEnd = "";
            string strFooterSeparator = "";
            LinkButton ctl = null;
            LiteralControl lit = null;
            bool bFirstLeft = false;
            bool bFirstFooter = false;

            ht = Utilities.ReadNavConfigValues("formatting");

            if (ht != null)
            {
                strLeftStart = ht["left_start"].ToString();
                strLeftEnd = ht["left_end"].ToString();
                strLeftSeparator = ht["left_separator_topnav"].ToString();
                strFooterStart = ht["footer_start"].ToString();
                strFooterEnd = ht["footer_end"].ToString();
                strFooterSeparator = ht["footer_separator"].ToString();
            }

            bFirstLeft = true;
            bFirstFooter = true;
            handler = new SingleTagSectionHandler();
            xmlLinks = Utilities.GetNavSettingsConfigNode("links");

            if (xmlLinks != null)
            {
                foreach (System.Xml.XmlNode node in xmlLinks.ChildNodes)
                {
                    if (node.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        ht = (Hashtable)handler.Create(null, null, node);

                        if ((string.Compare(ht["registered"].ToString(), "show", true) == 0 && ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "hide", true) == 0 && !ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "show", true) != 0 && string.Compare(ht["registered"].ToString(), "hide", true) != 0))
                        {
                            //MS added logic to check for role or participant type
                            if (ht["role"].Equals(string.Empty) || string.Compare(ht["role"].ToString(), Convert.ToString(Session["PaxRole"]), true) == 0)
                            {
                                if (string.Compare(ht["show_left"].ToString(), "true", true) == 0)
                                {
                                    if (bFirstLeft)
                                    {
                                        if (strLeftStart.Length > 0)
                                        {
                                            // output beginning markup for left nav before first link
                                            lit = new LiteralControl();
                                            lit.Text = strLeftStart;
                                            this.pnlNavLeft.Controls.Add(lit);
                                        }
                                        bFirstLeft = false;
                                    }
                                    else
                                    {
                                        lit = new LiteralControl();
                                        lit.Text = strLeftSeparator;
                                        this.pnlNavLeft.Controls.Add(lit);
                                    }

                                    ctl = new LinkButton();
                                    ctl.ID = string.Concat("ln_", node.Name);
                                    ctl.Text = ht["text"].ToString();
                                    ctl.ToolTip = ht["tooltip"].ToString();
                                    ctl.CommandName = ht["target"].ToString();

                                    try
                                    {
                                        if (ht["style"].ToString().Length > 0)
                                        {
                                            ctl.Attributes.Add("style", ht["style"].ToString());
                                        }
                                        else
                                        {
                                            ctl.Attributes.Add("style", string.Empty);
                                        }
                                    }
                                    catch
                                    {
                                        ctl.Attributes.Add("style", string.Empty);
                                    }

                                    try
                                    {
                                        if (ht["class"].ToString().Length > 0)
                                        {
                                            ctl.CssClass = ht["class"].ToString();
                                        }
                                        else
                                        {
                                            ctl.CssClass = "navigation";
                                        }
                                    }
                                    catch
                                    {
                                        ctl.CssClass = "navigation";
                                    }

                                    if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                    {
                                        ctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                    }
                                    else
                                    {
                                        ctl.Click += LinkButton_Click;
                                    }

                                    this.pnlNavLeft.Controls.Add(ctl);
                                }

                                if (string.Compare(ht["show_footer"].ToString(), "true", true) == 0)
                                {
                                    if (bFirstFooter)
                                    {
                                        if (strFooterStart.Length > 0)
                                        {
                                            // output beginning markup for footer nav before first link
                                            lit = new LiteralControl();
                                            lit.Text = strFooterStart;
                                            this.pnlNavFooter.Controls.Add(lit);
                                        }

                                        bFirstFooter = false;
                                    }
                                    else
                                    {
                                        lit = new LiteralControl();
                                        lit.Text = strFooterSeparator;
                                        this.pnlNavFooter.Controls.Add(lit);
                                    }

                                    ctl = new LinkButton();
                                    ctl.ID = string.Concat("bn_", node.Name);
                                    ctl.CssClass = "footer";
                                    ctl.Text = ht["text"].ToString();
                                    ctl.ToolTip = ht["tooltip"].ToString();
                                    ctl.CommandName = ht["target"].ToString();

                                    if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                    {
                                        ctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                    }
                                    else
                                    {
                                        ctl.Click += LinkButton_Click;
                                    }

                                    this.pnlNavFooter.Controls.Add(ctl);
                                }
                            }
                        }
                    }
                }
            }

            if ((!bFirstLeft) && (strLeftEnd.Length > 0) && (strLeftEnd.Length > 0))
            {
                // there were left nav links and ending markup exists
                lit = new LiteralControl();
                lit.Text = strLeftEnd;
                this.pnlNavLeft.Controls.Add(lit);
            }

            //FeedBack Link
            ImageButton imgctl = default(ImageButton);
            //xmlLinks = Utilities.GetNavSettingsConfigNode("FeedbackLink");

            if (xmlLinks != null)
            {
                foreach (System.Xml.XmlNode node in xmlLinks.ChildNodes)
                {
                    if (node.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        ht = (Hashtable)handler.Create(null, null, node);

                        if ((string.Compare(ht["registered"].ToString(), "show", true) == 0 && ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "hide", true) == 0 && !ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "show", true) != 0 && string.Compare(ht["registered"].ToString(), "hide", true) != 0))
                        {
                            //MS added logic to check for role or participant type
                            if (ht["role"].Equals(string.Empty) || string.Compare(ht["role"].ToString(), Convert.ToString(ContextManager.AttendeeType), true) == 0)
                            {
                                if (string.Compare(ht["show"].ToString(), "true", true) == 0)
                                {
                                    if (ht.ContainsKey("AddImage") && string.Compare(ht["AddImage"].ToString(), "true", true) == 0)
                                    {
                                        imgctl = new ImageButton();
                                        imgctl.ID = string.Concat("Imgbn_", node.Name);
                                        if (ht["class"].ToString().Length > 0)
                                        {
                                            imgctl.CssClass = ht["class"].ToString();
                                        }
                                        else
                                        {
                                            imgctl.CssClass = "footer";
                                        }
                                        imgctl.ImageUrl = ht["Imageurl"].ToString();
                                        imgctl.ToolTip = ht["tooltip"].ToString();
                                        imgctl.CommandName = ht["target"].ToString();
                                        imgctl.CommandName = ht["target"].ToString();
                                        if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                        {
                                            imgctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                        }
                                        else
                                        {
                                            imgctl.Click += LinkButton_Click;
                                        }
                                        //this.pnlFeedBack.Controls.Add(imgctl);
                                    }

                                    ctl = new LinkButton();
                                    ctl.ID = string.Concat("bn_", node.Name);
                                    if (ht["class"].ToString().Length > 0)
                                    {
                                        ctl.CssClass = ht["class"].ToString();
                                    }
                                    else
                                    {
                                        ctl.CssClass = "footer";
                                    }
                                    ctl.Text = " " + ht["text"].ToString();
                                    ctl.ToolTip = ht["tooltip"].ToString();
                                    ctl.CommandName = ht["target"].ToString();


                                    if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                    {
                                        ctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                    }
                                    else
                                    {
                                        ctl.Click += LinkButton_Click;
                                    }

                                    //this.pnlFeedBack.Controls.Add(ctl);
                                }
                            }
                        }
                    }
                }
            }

            //Split Nav Begin **
            ht = Utilities.ReadNavConfigValues("splitnav");
            topnavlink.Visible = Convert.ToBoolean(ht["ispageusingslitnav"]);
            if (topnavlink.Visible)
            {
                //now check if we need to add split navigation
                ht = Utilities.ReadNavConfigValues("splitformatting");
                if (ht != null)
                {
                    strLeftSeparator = ht["link_separator"].ToString();
                }

                //split nav Top-right hand corner
                bFirstLeft = true;
                handler = new SingleTagSectionHandler();
                xmlLinks = Utilities.GetNavSettingsConfigNode("splitnavlinks");
                foreach (System.Xml.XmlNode node in xmlLinks.ChildNodes)
                {
                    if (node.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        ht = (Hashtable)handler.Create(null, null, node);

                        if ((string.Compare(ht["registered"].ToString(), "show", true) == 0 && ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "hide", true) == 0 && !ContextManager.IsRegistered) || (string.Compare(ht["registered"].ToString(), "show", true) != 0 && string.Compare(ht["registered"].ToString(), "hide", true) != 0))
                        {
                            //MS added logic to check for role or participant type
                            if (ht["role"].Equals(string.Empty) || string.Compare(ht["role"].ToString(), Convert.ToString(ContextManager.AttendeeType), true) == 0)
                            {
                                if (string.Compare(ht["show_top"].ToString(), "true", true) == 0)
                                {
                                    if (bFirstLeft)
                                    {
                                        ctl = new LinkButton();
                                        ctl.ID = string.Concat("ln_", node.Name);
                                        ctl.Text = ht["text"].ToString();
                                        ctl.ToolTip = ht["tooltip"].ToString();
                                        ctl.CommandName = ht["target"].ToString();
                                        if (ht["class"].ToString().Length > 0)
                                        {
                                            ctl.CssClass = ht["class"].ToString();
                                        }
                                        else
                                        {
                                            ctl.CssClass = "navigation";
                                        }
                                        if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                        {
                                            ctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                        }
                                        else
                                        {
                                            ctl.Click += LinkButton_Click;
                                        }

                                        this.pnlSplitNav.Controls.Add(ctl);
                                        bFirstLeft = false;
                                    }
                                    else
                                    {
                                        //create separator
                                        if (strLeftSeparator.Length > 0)
                                        {
                                            lit = new LiteralControl();
                                            lit.Text = strLeftSeparator;
                                            this.pnlSplitNav.Controls.Add(lit);
                                        }
                                        ctl = new LinkButton();
                                        ctl.ID = string.Concat("ln_", node.Name);
                                        ctl.Text = ht["text"].ToString();
                                        ctl.ToolTip = ht["tooltip"].ToString();
                                        ctl.CommandName = ht["target"].ToString();
                                        if (ht["class"].ToString().Length > 0)
                                        {
                                            ctl.CssClass = ht["class"].ToString();
                                        }
                                        else
                                        {
                                            ctl.CssClass = "navigation";
                                        }
                                        if ((!string.IsNullOrEmpty(ht["type"].ToString())) && ht["type"].Equals(Convert.ToString("newwindow")))
                                        {
                                            ctl.OnClientClick = string.Concat("javascript: window.open(\"", ht["target"].ToString(), "\"); return false;");
                                        }
                                        else
                                        {
                                            ctl.Click += LinkButton_Click;
                                        }

                                        this.pnlSplitNav.Controls.Add(ctl);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Split Nav End **

            if ((!bFirstFooter) && (strFooterEnd.Length > 0) && (strFooterEnd.Length > 0))
            {
                // there were footer nav links and ending markup exists
                lit = new LiteralControl();
                lit.Text = strFooterEnd;
                this.pnlNavFooter.Controls.Add(lit);
            }
        }

        protected void LinkButton_Click(System.Object sender, System.EventArgs e)
        {
            Utilities.Navigate(((LinkButton)sender).CommandName, "MainMaster:LinkButton_Click");
        }

        private void LoadRegistrationNavigation()
        {
            System.Xml.XmlNode xmlLinks = default(System.Xml.XmlNode);
            SingleTagSectionHandler handler = default(SingleTagSectionHandler);
            Hashtable ht = default(Hashtable);
            Hashtable htLevels = default(Hashtable);
            int lngCurrentLevel = 0;
            string strStart = "";
            string strEnd = "";
            string strSeparator = "";
            LiteralControl lit = default(LiteralControl);
            bool bFirst = false;

            htLevels = Utilities.ReadNavConfigValues("registration_levels");

            if (htLevels != null)
            {
                ht = Utilities.ReadNavConfigValues("registration_formatting");

                if (ht != null)
                {
                    strStart = ht["start"].ToString();
                    strEnd = ht["end"].ToString();
                    strSeparator = ht["separator"].ToString();
                }

                ht = Utilities.ReadCustomConfig(string.Concat("pages/", Utilities.GetCurrentPage()));

                if (ht != null)
                {
                    lngCurrentLevel = Convert.ToInt32(htLevels[ht["reg_nav_level"]]);
                }
                else
                {
                    lngCurrentLevel = 0;
                }

                handler = new SingleTagSectionHandler();
                xmlLinks = Utilities.GetNavSettingsConfigNode("registration_links");
                bFirst = true;

                if (strStart.Length > 0)
                {
                    lit = new LiteralControl();
                    lit.Text = strStart;
                    this.pnlRegistrationNavigation.Controls.Add(lit);
                }

                var ul = new HtmlGenericControl("ul");
                ul.Attributes["class"] = "nav nav-pills nav-justified";

                foreach (XmlNode node in xmlLinks.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        ht = (Hashtable)handler.Create(null, null, node);

                        if ((bFirst) || strSeparator.Length <= 0)
                        {
                            bFirst = false;
                        }
                        var li = new HtmlGenericControl("li");

                        if (Convert.ToInt32(htLevels[ht["level"]]) < lngCurrentLevel)
                        {
                            li.Attributes["class"] = "prior";
                            var link = new LinkButton();
                            link.Text = ht["text"].ToString();
                            link.ToolTip = ht["tooltip"].ToString();
                            link.CommandName = ht["target"].ToString();
                            link.Attributes.Add("data_CommandName", ht["target"].ToString());
                            link.Click += LinkButton_Click;
                            li.Controls.Add(link);
                        }
                        else if (Convert.ToInt32(htLevels[ht["level"]]) == lngCurrentLevel)
                        {
                            li.Attributes["class"] = "active";
                            var link = new LinkButton();
                            link.Text = ht["text"].ToString();
                            link.ToolTip = ht["tooltip"].ToString();
                            link.CommandName = ht["target"].ToString();
                            link.Attributes.Add("data_CommandName", ht["target"].ToString());
                            link.Click += LinkButton_Click;
                            li.Controls.Add(link);
                        }
                        else
                        {
                            li.Attributes["class"] = "future";
                            var link = new HtmlGenericControl("a");
                            link.InnerText = ht["text"].ToString();
                            link.Attributes["title"] = ht["tooltip"].ToString();
                            link.Attributes["class"] = "disabled";
                            link.Attributes["href"] = "#";
                            li.Controls.Add(link);
                        }
                        ul.Controls.Add(li);
                    }
                }

                this.pnlRegistrationNavigation.Controls.Add(ul);
                if (strEnd.Length > 0)
                {
                    lit = new LiteralControl();
                    lit.Text = strEnd;
                    this.pnlRegistrationNavigation.Controls.Add(lit);
                }
            }
        }

    }
}