using System.Collections.Generic;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/transportation")]
    public class TransportationController : ApiController
    {
        [HttpGet]
        [Route("{participantId:int}")]
        public IHttpActionResult GetTransportation(int participantId)
        {
            // TODO: Wire to PCentralLib.dll — transportation details lookup
            var placeholder = new Dictionary<string, object>
            {
                ["ParticipantID"] = participantId
            };
            return Ok(placeholder);
        }
    }
}
