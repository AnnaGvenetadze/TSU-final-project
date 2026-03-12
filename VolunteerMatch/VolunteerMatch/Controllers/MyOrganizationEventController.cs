using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// TODO: Add endpoints for creating, updating, and listing events created by the organization
//POST api/organizations/me/events
//PUT api/organizations/me/events/{id}
//GET api/organizations/me/events/{id} -> კონკრეტული ივენთი დეტალურად
//GET api/organizations/me/events -> ქარდების სია
//GET api/organizations/me/matches/events
namespace VolunteerMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyOrganizationEventController : BaseController
    {

    }
}
