using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Threading;
using ITALib;
using PCentralLib.tasks;
using PCentralLib;
using PCentralLib.credit;
using PCentralLib.logging;
using System.Configuration;
using PCentralLib.CardProcessService;

namespace FlyITA
{
    public partial class DiagnosticsDefault : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            VersionGrid.DataSource = CreateDataSource();
            VersionGrid.DataBind();

            lblVersion.Text = Utilities.publish_date.ToString() + " - " + Utilities.getRole()  ;
        }

        
        public ICollection CreateDataSource()
        {

            DataTable dt = default(DataTable);
            DataRow dr = default(DataRow);
            int i = 0;

            //create a DataTable
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("AssemblyName", typeof(string)));
            dt.Columns.Add(new DataColumn("File DateTime", typeof(string)));
            dt.Columns.Add(new DataColumn("Version", typeof(string)));
            // dt.Columns.Add(New DataColumn("StringValue", GetType(String)))
            Assembly[] myAssemblies = Thread.GetDomain().GetAssemblies();

            // Get the dynamic assembly named 'MyAssembly'. 
            Assembly myAssembly = null;
            //Dim i As Integer
            for (i = 0; i <= myAssemblies.Length - 1; i++)
            {
                if (myAssemblies[i].GetName().Name.IndexOf("ITA") >= 0 || myAssemblies[i].GetName().Name.IndexOf("PCentral") >= 0 || myAssemblies[i].GetName().Name.IndexOf("Telerik") >= 0)
                {
                    myAssembly = myAssemblies[i];
                    dr = dt.NewRow();
                    dr[0] = myAssembly.FullName;
                    dr[1] = assemblyLastWriteTime(myAssembly);
                    dr[2] = myAssembly.GetName().Version.ToString();
                    dt.Rows.Add(dr);
                }
            }
            //return a DataView to the DataTable
            return new DataView(dt);

        }

        private DateTime assemblyLastWriteTime(System.Reflection.Assembly a)
        {
            try
            {
                return System.IO.File.GetLastWriteTime(a.Location);
            }
            catch
            {
                return DateTime.MaxValue;
            }
        }
    }
}