using System.Web.Http;
using PCentralLib;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/persons")]
    public class PersonsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetPerson(int id)
        {
            var person = PCentralLib.WebReg.PCentralWebRegFunctions.GetPersonByID(ConnectionHelper.PerformanceCentral, id);
            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [HttpGet]
        [Route("{id:int}/contacts")]
        public IHttpActionResult GetContacts(int id)
        {
            var contacts = PCentralPersonContactNumber.GetByPersonID(ConnectionHelper.PerformanceCentral, id);
            return Ok(contacts ?? new PCentralEntityList<PCentralPersonContactNumber>());
        }
    }
}
