using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// TODO: Add endpoints for listing events that the volunteer has matched with or favourited
// GET api/volunteers/me/matches/events
// GET api/volunteers/me/favourites/events
namespace VolunteerMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyVolunteerEventController : ControllerBase
    {
    }
}
