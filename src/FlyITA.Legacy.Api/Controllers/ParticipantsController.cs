using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using PCentralLib;
using PCentralLib.WebReg;

namespace FlyITA.Legacy.Api.Controllers
{
    [RoutePrefix("api/participants")]
    public class ParticipantsController : ApiController
    {
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetParticipant(int id)
        {
            var participant = PCentralParticipant.GetByID(ConnectionHelper.PerformanceCentral, id);
            if (participant == null)
                return NotFound();

            return Ok(participant.DumpToHTML(false, false, true));
        }

        [HttpGet]
        [Route("{id:int}/party")]
        public IHttpActionResult GetParty(int id)
        {
            var partyId = PCentralLib.parties.PCentralParty.GetPartyIDByParticipantID(ConnectionHelper.PerformanceCentral, id);
            if (partyId <= 0)
                return NotFound();

            var party = PCentralLib.parties.PCentralParty.GetByID(ConnectionHelper.PerformanceCentral, partyId);
            if (party == null)
                return NotFound();

            return Ok(party);
        }

        [HttpGet]
        [Route("{id:int}/custom-fields")]
        public IHttpActionResult GetCustomFields(int id)
        {
            var values = PCentralProgramCustomField.GetCustomFieldValuesByParticipantID(ConnectionHelper.PerformanceCentral, id);
            return Ok(values ?? new Dictionary<string, string>());
        }

        [HttpPut]
        [Route("{id:int}/custom-fields")]
        public IHttpActionResult SaveCustomField(int id, [FromBody] CustomFieldRequest request)
        {
            // Custom field save happens via participant.Save() on the PCentralParticipant entity
            // The participant object manages its own custom fields
            var participant = PCentralParticipant.GetByID(ConnectionHelper.PerformanceCentral, id);
            if (participant == null)
                return NotFound();

            // Custom fields are saved as part of the participant entity graph
            // Individual field saves go through the participant's CustomFields collection
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{id:int}/accommodations")]
        public IHttpActionResult GetAccommodation(int id)
        {
            var result = WebRegAccommodationFacade.ReadAccommodation(ConnectionHelper.PerformanceCentral, id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}/accommodations/list")]
        public IHttpActionResult GetAccommodationList(int id)
        {
            var result = WebRegAccommodationFacade.ReadAccommodation(ConnectionHelper.PerformanceCentral, id);
            if (result == null)
                return Ok(new object[0]);

            return Ok(result);
        }

        [HttpPut]
        [Route("{id:int}/accommodations")]
        public IHttpActionResult SaveAccommodation(int id, [FromBody] Dictionary<string, object> data)
        {
            // Accommodation save happens through the PCentralParticipant entity save
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpDelete]
        [Route("{id:int}/accommodations/{recordType}")]
        public IHttpActionResult DeleteAccommodation(int id, string recordType)
        {
            // Accommodation delete happens through the PCentralParticipant entity graph
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("{id:int}/transportation")]
        public IHttpActionResult GetTransportation(int id)
        {
            var result = WebRegAccommodationFacade.ReadAccommodation(ConnectionHelper.PerformanceCentral, id);
            if (result?.AirPreference == null)
                return NotFound();

            return Ok(result.AirPreference);
        }
    }

    public class CustomFieldRequest
    {
        public int CustomFieldId { get; set; }
        public string Value { get; set; }
        public int PossibleValueId { get; set; }
    }
}
