using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using System.Web;

namespace FlyITA
{
    public abstract class PageBase : System.Web.UI.Page, IPageViewBase
    {
        public abstract System.Web.UI.WebControls.Label UserMessages { get; set; }
        protected bool has_error = false;

        public void show_system_error()
        {
            string err = HttpContext.Current.Request.Headers["System_Error_Message"] ?? "";
            if (!string.IsNullOrWhiteSpace(err))
            {
                this.UserMessages.Text += err;
                this.has_error = true;
            }
            else if (HttpContext.Current.Request.Params["syserror"] != null)
            {
                this.UserMessages.Text += Global.error_message_500;
                this.has_error = true;
            }
        }

        public CustomFieldControlCollection CustomFieldControls
        {
            get
            {
                CustomFieldControlCollection cfcCollection = default(CustomFieldControlCollection);

                cfcCollection = new CustomFieldControlCollection();
                CustomFieldControlsHelper(this, cfcCollection);

                return cfcCollection;
            }
        }

        private void CustomFieldControlsHelper(Control container, CustomFieldControlCollection cfcCollection)
        {
            foreach (Control ctl in container.Controls)
            {
                if (ctl is ICustomFieldControl && !((ICustomFieldControl)ctl).WriteOnce)
                {
                    cfcCollection.Add((ICustomFieldControl)ctl);
                }
                else if (ctl.HasControls())
                {
                    CustomFieldControlsHelper(ctl, cfcCollection);
                }
            }
        }

        public void AddDynamicCustomControl(System.Web.UI.HtmlControls.HtmlGenericControl ph, string cfName, bool required, string id, bool visible)
        {
            var cf = (ucCustomField)LoadControl("~\\UserControls\\ucCustomField.ascx");

            cf.CustomFieldName = cfName;
            cf.ID = id;
            cf.Visible = visible;
            cf.IsRequired = required;

            ph.Controls.Add(cf);
        }
    }
}
