using System.Web.Http;
using PCentralLib.WebReg;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/transportation")]
    public class TransportationController : ApiController
    {
        [HttpGet]
        [Route("{participantId:int}")]
        public IHttpActionResult GetTransportation(int participantId)
        {
            var result = WebRegAccommodationFacade.ReadAccommodation(ConnectionHelper.PerformanceCentral, participantId);
            if (result?.AirPreference == null)
                return NotFound();

            return Ok(result.AirPreference);
        }
    }
}
