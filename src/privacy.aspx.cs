using PCentralLib.programs;
using System;
using System.Data;
using ITALib;
using ITALib.DataAccess;

namespace FlyITA
{
    public partial class privacy : System.Web.UI.Page
    {
        //protected void Page_Load(object sender, System.EventArgs e)
        //{
        //    PCentralProgram program = PCentralProgram.GetByID(Utilities.GetEnvironmentConnection(), ContextManager.ProgramID.ToString());

        //    //first check if CMS control has content
        //    if (!CMSContent())
        //    {
        //        if (program.TravelHeadquartersName.Length > 0)
        //        {
        //            this.lblClientName.Text = program.TravelHeadquartersName;
        //            this.lblTravelHQ.Text = program.TravelHeadquartersName;
        //            this.hlProgramurl.Text = program.WebRegistrationSiteUrl;
        //            this.hlProgramurl.NavigateUrl = program.WebRegistrationSiteUrl;
        //            this.hpMailto.NavigateUrl = string.Concat("mailto:", program.FromEmailAddress);
        //            this.hpMailto.Text = program.FromEmailAddress;
        //        }
        //        else
        //        {
        //            //some generic message
        //            //BU needs to provide what the message should be
        //        }
        //    }
        //    else
        //    {
        //        this.pnlGenerigMsg.Visible = false;
        //    }
        //}

        //private bool CMSContent()
        //{
        //    DataSet dsContent = null;

        //    using (DB mssql = new DB(Utilities.GetEnvironmentConnectionWebRegAdmin()))
        //    {
        //        dsContent = mssql.Get_DataSet("dbo.spCMSContentForDisplaySelect", ContextManager.ProgramID, Utilities.GetCurrentPage(), this.UcCMS1.ID, ContextManager.ParticipantID);
        //    }

        //    return (dsContent.HasRows(0) && dsContent.Tables[0].Rows[0][0].ToString().Length > 0);
        //}

        //public privacy()
        //{
        //    Load += Page_Load;
        //}
    }

}