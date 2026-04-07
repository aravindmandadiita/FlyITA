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
            // Email templates are loaded from the database via PCentralLib internals
            // The legacy app builds email bodies using CustomEmails.cs utility methods
            // which compose templates from participant/person/program data
            // TODO: Expose template lookup when exact DLL method is identified
            return Ok(new { value = "" });
        }

        [HttpGet]
        [Route("email-body/{key}")]
        public IHttpActionResult GetEmailBody(string key, [FromUri] int programId)
        {
            // Email body composition is a multi-step process in CustomEmails.cs
            // It uses participant data, custom fields, and template replacement
            // TODO: Expose email body builder when exact DLL method is identified
            return Ok(new { value = "" });
        }
    }
}
