using System.Collections.Generic;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/persons")]
    public class PersonsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetPerson(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralPerson.Get(id)
            var placeholder = new Dictionary<string, object>
            {
                ["PersonID"] = id,
                ["FirstName"] = "",
                ["LastName"] = "",
                ["Email"] = ""
            };
            return Ok(placeholder);
        }

        [HttpGet]
        [Route("{id:int}/contacts")]
        public IHttpActionResult GetContacts(int id)
        {
            // TODO: Wire to PCentralLib.dll — contact number lookup
            return Ok(new List<Dictionary<string, object>>());
        }
    }
}
