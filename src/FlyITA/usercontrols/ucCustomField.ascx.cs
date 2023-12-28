using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using ITALib;
using System.Linq;
using PCentralLib.custom_fields;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace FlyITA
{
    public partial class ucCustomField : UserControl, ICustomFieldControl
    {
        private int _ParticipantID = ContextManager.ParticipantID;
        private string _CustomFieldName = string.Empty;
        private string _CustomLabel = string.Empty;

        private string _LabelTerminator = ":";

        private void Page_Init(object sender, System.EventArgs e)
        {
            if (ViewState["onClientLoad"].toStringOr() != string.Empty)
            {
                string scrpt = ViewState["onClientLoad"].ToString();

                scrpt = scrpt.Replace("[control_id]", this.ClientIDControl);
                scrpt = scrpt.Replace("[label_id]", this.ClientIDLabel);
                scrpt = scrpt.Replace("[required_id]", this.ClientIDRequired);
                scrpt = scrpt.Replace("[id]", this.ClientID);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "Load_" + this.ClientID, scrpt, true);
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblCF.Text = string.Concat(UILabel, _LabelTerminator);
            }
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            if (IsRequired)
            {
                dvRequiredField.Attributes["class"] = "form-group required";
            }
            else
            {
                dvRequiredField.Attributes["class"] = "form-group";
                txtCF.Attributes.Remove("required");
                ddlCF.Attributes.Remove("required");
            }

            if (!Page.IsPostBack)
            {
                txtCF.Visible = !IsDropDown;
                ddlCF.Visible = IsDropDown;

                lblCF.AssociatedControlID = IsDropDown ? txtCF.ID : ddlCF.ID;

                if (IsDropDown && AutoLoadValueList && ddlCF.DataSource == null)
                {
                    DataSet ds = PCentralProgramCustomField.GetCustomFieldPossibleValues(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, CustomFieldName.toStringOr(CustomFieldID.ToString()));

                    DataTable dt = ds.Tables[0];

                    Utilities.LoadList((ListControl)this.ddlCF, ref dt, "CustomFieldPossibleValueID", "Value", "Select an Option");
                }
            }
            if (Utilities.IsNothingNullOrEmptyorZero(lblCF.AssociatedControlID))
            {
                lblCF.AssociatedControlID = IsDropDown ? txtCF.ID : ddlCF.ID;
            }

        }

        public string ClientIDControl
        {
            get
            {
                if (this.WriteOnce)
                {
                    return txtCF.ClientID;
                }
                if (this.IsDropDown)
                {
                    return (ddlCF.ClientID);
                }
                return txtCF.ClientID;
            }
        }

        public string ClientIDLabel
        {
            get
            {
                return lblCF.ClientID;
            }
        }

        public string ClientIDRequired
        {
            get
            {
                return dvRequiredField.ClientID;
            }
        }

        public int CustomFieldID
        {
            get
            {
                int id = ViewState["CustomFieldID"].toInt();

                if (id == 0 && string.IsNullOrEmpty(CustomFieldName))
                {
                    id = PCentralProgramCustomField.GetCustomFieldIDFromName(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, CustomFieldName);

                    if (id > 0) ViewState["CustomFieldID"] = id;
                }

                return id;
            }
            set { ViewState["CustomFieldID"] = value; }
        }

        public int GetCustomFieldID(bool look_up_if_missing = false)
        {
            int id = ViewState["CustomFieldID"].toInt();

            if (id == 0 && look_up_if_missing && string.IsNullOrEmpty(CustomFieldName))
            {
                id = PCentralProgramCustomField.GetCustomFieldIDFromName(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID, CustomFieldName);

                if (id > 0) ViewState["CustomFieldID"] = id;
            }

            return id;
        }

        public int CustomFieldPossibleValueID
        {
            get { return this.ddlCF.SelectedValue.toInt(); }
            set
            {
                if (!(Utilities.IsNothingNullOrEmptyorZero(ddlCF.Items.FindByValue(value.ToString()))))
                {
                    this.ddlCF.SelectedValue = value.ToString();
                }
                else
                {
                    this.ddlCF.SelectedIndex = 0;
                }


            }
        }

        public DataTable dtCustomFieldPossibleValues
        {
            set
            {
                this.txtCF.Visible = false;
                this.ddlCF.Visible = true;

                Utilities.LoadList((ListControl)this.ddlCF, ref value, "CustomFieldPossibleValueID", "Value", "Select an Option", false, string.Concat("CustomFieldID = ", ViewState["CustomFieldID"]));

                // already sorted
            }
        }

        public bool HasDataSource
        {
            get
            {
                return this.ddlCF.DataSourceObject != null;
            }
        }

        public string Value
        {
            get { return this.txtCF.Text; }
            set
            {
                this.ddlCF.Visible = false;
                if (WriteOnce)
                {
                    this.lblCFReadOnly.Text = value;
                }
                else
                {
                    this.txtCF.Visible = true;
                    this.txtCF.Text = value;
                }
            }
        }

        public int SelectedIndex
        {
            get { return ddlCF.SelectedIndex; }
            set { ddlCF.SelectedIndex = value; }
        }

        public ListItemCollection Items
        {
            get { return ddlCF.Items; }
        }

        public string SelectedItemText
        {
            get
            {
                if (this.ddlCF.Visible == true)
                {
                    return this.ddlCF.SelectedItem.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { this.CustomFieldPossibleValueID = Convert.ToInt32(this.ddlCF.Items.FindByText(value).Value); }
        }

        public bool AllowPostBack
        {
            get { return ddlCF.AutoPostBack; }
            set { ddlCF.AutoPostBack = value; }
        }

        [System.ComponentModel.Category("Appearance")]
        public string CustomFieldValue
        {
            get
            {
                if (this.txtCF.Visible)
                {
                    return this.Value;
                }
                else
                {
                    return this.CustomFieldPossibleValueID.ToString();
                }
            }
            set
            {
                if (this.txtCF.Visible)
                {
                    this.Value = value;
                }
                else
                {
                    this.CustomFieldPossibleValueID = Convert.ToInt32(value);
                }
            }
        }

        [System.ComponentModel.Category("")]
        public int ParticipantID
        {
            get { return _ParticipantID; }
            set { _ParticipantID = value; }
        }

        public bool WriteOnce
        {
            get { return this.lblCFReadOnly.Visible; }
            set
            {
                if (value)
                {
                    // in case Value property was already set
                    this.lblCFReadOnly.Text = this.txtCF.Text;
                    this.txtCF.Visible = false;
                }
                this.lblCFReadOnly.Visible = value;
            }
        }

        public string UILabel
        {
            get
            {
                if (_CustomLabel.Length > 0)
                {
                    return _CustomLabel;
                }
                else
                {
                    return _CustomFieldName;
                }
            }
        }

        [System.ComponentModel.Category("Behavior")]
        public string CustomFieldName
        {
            get { return _CustomFieldName; }
            set { _CustomFieldName = value; }
        }

        [System.ComponentModel.Category("Appearance")]
        public string CustomLabel
        {
            get { return _CustomLabel; }
            set { _CustomLabel = value; }
        }

        [System.ComponentModel.DefaultValue(":")]
        [System.ComponentModel.Category("Appearance")]
        public string LabelTerminator
        {
            get { return _LabelTerminator; }
            set { _LabelTerminator = value; }
        }


        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Category("Behavior")]
        public bool IsRequired
        {
            get
            {
                return ViewState["isRequired"].toBool(true);
            }
            set
            {
                ViewState["isRequired"] = value;
            }
        }

        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.Category("Behavior")]
        public bool IsVisible
        {
            //// The label's parent is dvRequiredField, and it's parent is the div
            //// that it was placed in.
            get
            {
                try
                {
                    return this.dvRequiredField.Parent.Parent.Parent.Visible;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    this.dvRequiredField.Parent.Parent.Parent.Visible = value;
                }
                catch
                {
                    //// do nothing
                }
            }
        }

        [System.ComponentModel.Category("Behavior")]
        public int MaxLength
        {
            get { return this.txtCF.MaxLength; }
            set { this.txtCF.MaxLength = value; }
        }

        [System.ComponentModel.Category("Behavior")]
        public string OnClientChange
        {
            get
            {
                IEnumerator keys = this.txtCF.Attributes.Keys.GetEnumerator();

                while (keys.MoveNext())
                {
                    if ((String)keys.Current == "onchange")
                    {
                        return this.txtCF.Attributes["onchange"];
                    }
                }

                return string.Empty;
            }
            set
            {
                this.txtCF.Attributes["onchange"] = value;
                this.ddlCF.Attributes["onchange"] = value;
            }
        }

        [System.ComponentModel.Category("Behavior")]
        public string OnClientPageLoad
        {
            get
            {
                return ViewState["onClientLoad"].toStringOr();
            }
            set
            {
                ViewState["onClientLoad"] = value;
            }
        }

        public bool IsTextBox
        {
            get { return (ViewState["DisplayType"].ToString() == "textbox"); }
            set
            {
                if (value)
                {
                    ViewState["DisplayType"] = "textbox";
                }
            }
        }

        [System.ComponentModel.Category("Behavior")]
        public bool IsDropDown
        {
            get { return (ViewState["DisplayType"].toStringOr() == "dropdown"); }
            set
            {
                if (value)
                {
                    ViewState["DisplayType"] = "dropdown";
                }
                else
                {
                    ViewState["DisplayType"] = "textbox";
                }
            }
        }

        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.Category("Behavior")]
        public bool AutoLoadValueList
        {
            get { return ViewState["AutoLoadValueList"].toBool(false); }
            set
            {
                ViewState["AutoLoadValueList"] = value;
            }
        }

        public ucCustomField()
        {
            Load += Page_Load;
            Init += Page_Init;
        }

        public string CustomFieldText
        {
            get
            {
                if (this.IsDropDown)
                {
                    return (ddlCF.SelectedItem.Text);
                }
                return txtCF.Text;
            }
        }
    }
}