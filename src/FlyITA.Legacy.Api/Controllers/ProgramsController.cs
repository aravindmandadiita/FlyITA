using System.Collections.Generic;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/programs")]
    public class ProgramsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetProgram(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralProgram.Get(id)
            var placeholder = new Dictionary<string, object>
            {
                ["ProgramID"] = id,
                ["ProgramName"] = "",
                ["ITAProgNbr"] = ""
            };
            return Ok(placeholder);
        }
    }
}
