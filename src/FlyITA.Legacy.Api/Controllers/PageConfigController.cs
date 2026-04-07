using System.Collections.Generic;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/page-config")]
    public class PageConfigController : ApiController
    {
        [HttpGet]
        [Route("{pageName}")]
        public IHttpActionResult GetPageConfig(string pageName, [FromUri] string programNumber)
        {
            // TODO: Wire to PCentralLib.dll — page configuration lookup
            var placeholder = new Dictionary<string, object>
            {
                ["PageName"] = pageName,
                ["ProgramNumber"] = programNumber ?? ""
            };
            return Ok(placeholder);
        }
    }
}
