using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly EventsService _eventService;

        public EventsController(EventsService eventService)
        {
            _eventService = eventService
                ?? throw new ArgumentNullException(nameof(eventService));
        }


        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                var events = await _eventService.GetEventsAsync(page, pageSize);

                return Ok(events);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }


        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            try
            {
                var eventDetails = await _eventService.GetEventByIdAsync(eventId);

                return Ok(eventDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }
    }
}