using System.IO;
using System.Web.UI.WebControls;

namespace FlyITA
{
    public partial class ImageListing : PageBase
    {

        #region " ### Page Events ### "

        //protected void Page_Load(object sender, System.EventArgs e)
        //{
        //    var path = "~/CMSFiles/images";
        //    var serverpath = Server.MapPath(path);
        //    var dir = new DirectoryInfo(serverpath);
        //    var json = "[";

        //    var count = 0;
        //    foreach (var flInfo in dir.GetFiles())
        //    {
        //        var filestr = "";
        //        if (count != 0)
        //            filestr += ",";
        //        filestr += "{";
        //        filestr += "\"image\": \"CMSFiles/images/";
        //        filestr += flInfo.Name;
        //        filestr += "\", \"thumb\": \"../../../../../CMSFiles/images/";
        //        filestr += flInfo.Name;
        //        filestr += "\", \"folder\": \"";
        //        filestr += dir.Name;
        //        filestr += "\"}";
        //        json += filestr;
        //        count++;
        //    }
        //    json += "]";

        //    Response.Clear();
        //    Response.ContentType = "application/json; charset=utf-8";
        //    Response.Write(json);
        //    Response.End();

        //}

        #endregion

        #region " ### Public Overridden Methods ### "

        public override Label UserMessages
        {
            get { return new Label(); }

            set { }
        }

        #endregion
    }
}