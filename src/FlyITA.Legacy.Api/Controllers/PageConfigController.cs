using System.Web.Http;
using PCentralLib.WebReg;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/page-config")]
    public class PageConfigController : ApiController
    {
        [HttpGet]
        [Route("{pageName}")]
        public IHttpActionResult GetPageConfig(string pageName, [FromUri] string programNumber)
        {
            // Page configuration is handled via PCentralWebRegDAL and program settings
            var dal = new PCentralWebRegDAL();
            // Configuration comes from the program's custom field setup and page settings
            // TODO: Wire specific page config lookup when exact method is confirmed
            return Ok(new { PageName = pageName, ProgramNumber = programNumber ?? "" });
        }
    }
}
