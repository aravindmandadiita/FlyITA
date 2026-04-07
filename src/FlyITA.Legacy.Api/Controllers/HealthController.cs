using System.Net.Http;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "healthy", service = "FlyITA.Legacy.Api" });
        }
    }
}
