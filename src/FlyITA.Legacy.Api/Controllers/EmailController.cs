using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api")]
    public class EmailController : ApiController
    {
        [HttpGet]
        [Route("email-templates/{name}")]
        public IHttpActionResult GetEmailTemplate(string name, [FromUri] int programId)
        {
            // TODO: Wire to PCentralLib.dll — email template lookup
            return Ok(new { value = "" });
        }

        [HttpGet]
        [Route("email-body/{key}")]
        public IHttpActionResult GetEmailBody(string key, [FromUri] int programId)
        {
            // TODO: Wire to PCentralLib.dll — email body lookup
            return Ok(new { value = "" });
        }
    }
}
