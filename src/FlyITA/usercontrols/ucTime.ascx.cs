using ITALib;
using System;
using System.Web.UI;

namespace FlyITA.usercontrols
{
    public partial class ucTime : System.Web.UI.UserControl
    {
        //private bool blnDisplaySeconds = false;

        //private string strUserLabel;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            ITATimeCtrlHr.Attributes.Add("onchange", "return chkHour(document.getElementById('" + this.ITATimeCtrlHr.ClientID + "'));");
            ITATimeCtrlMn.Attributes.Add("onchange", "return chkMinutes(document.getElementById('" + this.ITATimeCtrlMn.ClientID + "'));");

            Page.ClientScript.RegisterClientScriptInclude("ucTime_script", Page.ResolveClientUrl("scripts/ucTime.js"));
        }

        public string Text
        {
            get
            {
                string strTime = null;

                if (ITATimeCtrlHr.Text.Length == 0 && ITATimeCtrlMn.Text.Length == 0 && ITATimeCtrlTimeType.SelectedIndex == 0)
                {
                    //strTime = DEFAULT_DATETIME_VALUE
                    // returning default time was interfering with required field validation
                    strTime = string.Empty;
                }
                else
                {
                    strTime = string.Concat(ITATimeCtrlHr.Text, ":", ITATimeCtrlMn.Text, ":00 ", ITATimeCtrlTimeType.SelectedItem.Value);
                }

                return strTime;
            }
            set
            {
                value = (Convert.ToDateTime(value)).ToLongTimeString();

                if (!value.isEmpty(true))
                {
                    string[] arrTime = value.Split(new[] { ':' });
                    ITATimeCtrlHr.Text = arrTime[0];
                    ITATimeCtrlMn.Text = arrTime[1];
                    ITATimeCtrlTimeType.ClearSelection();
                    ITATimeCtrlTimeType.Items.FindByValue(value.Substring(value.LastIndexOf(":") + 4, 2)).Selected = true;
                }
                else
                {
                    ITATimeCtrlHr.Text = string.Empty;
                    ITATimeCtrlMn.Text = string.Empty;
                    ITATimeCtrlTimeType.SelectedIndex = 0;
                }
            }
        }

        public int Hour
        {
            get { return Convert.ToInt32(ITATimeCtrlHr.Text); }
            set { ITATimeCtrlMn.Text = value.ToString(); }
        }

        public int Minutes
        {
            get { return Convert.ToInt32(ITATimeCtrlMn.Text); }
            set { ITATimeCtrlMn.Text = value.ToString(); }
        }

        public bool Disabled
        {
            set
            {
                ITATimeCtrlHr.ReadOnly = value;
                ITATimeCtrlMn.ReadOnly = value;
                ITATimeCtrlTimeType.Enabled = value;
            }
        }
    }
}