using System.Collections.Generic;
using System.Web.Http;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/participants")]
    public class ParticipantsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetParticipant(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParticipant.Get(id)
            var placeholder = new Dictionary<string, object>
            {
                ["ParticipantID"] = id,
                ["FirstName"] = "",
                ["LastName"] = "",
                ["Email"] = ""
            };
            return Ok(placeholder);
        }

        [HttpGet]
        [Route("{id:int}/party")]
        public IHttpActionResult GetParty(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParty.GetByParticipant(id)
            var placeholder = new Dictionary<string, object>
            {
                ["PartyID"] = 0,
                ["ParticipantID"] = id
            };
            return Ok(placeholder);
        }

        [HttpGet]
        [Route("{id:int}/custom-fields")]
        public IHttpActionResult GetCustomFields(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralProgramCustomField.GetValues(id)
            return Ok(new List<Dictionary<string, object>>());
        }

        [HttpPut]
        [Route("{id:int}/custom-fields")]
        public IHttpActionResult SaveCustomField(int id, [FromBody] CustomFieldRequest request)
        {
            // TODO: Wire to PCentralLib.dll — PCentralProgramCustomField.Save(...)
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{id:int}/accommodations")]
        public IHttpActionResult GetAccommodation(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParticipant.GetAccommodation(id)
            var placeholder = new Dictionary<string, object>
            {
                ["ParticipantID"] = id
            };
            return Ok(placeholder);
        }

        [HttpGet]
        [Route("{id:int}/accommodations/list")]
        public IHttpActionResult GetAccommodationList(int id)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParticipant.GetAccommodationList(id)
            return Ok(new List<Dictionary<string, object>>());
        }

        [HttpPut]
        [Route("{id:int}/accommodations")]
        public IHttpActionResult SaveAccommodation(int id, [FromBody] Dictionary<string, object> data)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParticipant.SaveAccommodation(...)
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}/accommodations/{recordType}")]
        public IHttpActionResult DeleteAccommodation(int id, string recordType)
        {
            // TODO: Wire to PCentralLib.dll — PCentralParticipant.DeleteAccommodation(...)
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{id:int}/transportation")]
        public IHttpActionResult GetTransportation(int id)
        {
            // TODO: Wire to PCentralLib.dll — get transportation details
            var placeholder = new Dictionary<string, object>
            {
                ["ParticipantID"] = id
            };
            return Ok(placeholder);
        }
    }

    public class CustomFieldRequest
    {
        public int CustomFieldId { get; set; }
        public string Value { get; set; }
        public int PossibleValueId { get; set; }
    }
}
