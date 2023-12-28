using System.Web.UI;

public abstract class PageBase : System.Web.UI.Page, IPageViewBase
{
    public abstract System.Web.UI.WebControls.Label UserMessages { get; set; }

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

    private void CustomFieldControlsHelper(System.Web.UI.Control container, CustomFieldControlCollection cfcCollection)
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

    /// <summary>
    /// Adds Custom Filed control to a specified PlaceHolder
    /// </summary>
    /// <param name="ph"></param>
    /// <param name="cfName"></param>
    /// <param name="required"></param>
    /// <param name="id"></param>
    /// <param name="visible"></param>
    /// <param name="requiredstyle"></param>
    /// <param name="labelstyle"></param>
    /// <param name="controlstyle"></param>
    /// <param name="valuestyle"></param>
    /// <remarks></remarks>
    public void AddDynamicCustomControl(System.Web.UI.HtmlControls.HtmlGenericControl ph, string cfName, bool required, string id, bool visible)
    {
        FlyITA.ucCustomField cf = (FlyITA.ucCustomField)LoadControl("~\\UserControls\\ucCustomField.ascx");

        cf.CustomFieldName = cfName;
        cf.ID = id;
        cf.Visible = visible;
        cf.IsRequired = required;

        ph.Controls.Add(cf);
    }
}