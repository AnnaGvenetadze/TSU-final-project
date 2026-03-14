using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
// TODO: Add endpoints for listing events and getting event details
// GET api/events -> List<EventCardDto>
// GET api/events/{id} -> EventDetailsDto
namespace VolunteerMatch.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
    }
}
