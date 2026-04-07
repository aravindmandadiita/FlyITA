using System.Web.Http;
using PCentralLib;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/programs")]
    public class ProgramsController : ApiController
    {
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetProgram(string id)
        {
            var program = PCentralLib.WebReg.PCentralWebRegFunctions.GetProgram(ConnectionHelper.PerformanceCentral, id);
            if (program == null)
                return NotFound();

            return Ok(program);
        }
    }
}
