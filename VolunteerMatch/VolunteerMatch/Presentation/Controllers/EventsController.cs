using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize(Roles = $"{UserRoles.Volunteer},{UserRoles.Organization}")]
    public class EventsController : BaseController
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
                if (CurrentUserRole == UserRoles.Volunteer)
                {
                    var volunteerEvents = await _eventService
                        .GetEventsForVolunteerAsync(CurrentUserId, page, pageSize);

                    return Ok(volunteerEvents);
                }

                var organizationEvents = await _eventService
                    .GetEventsForOrganizationAsync(page, pageSize);

                return Ok(organizationEvents);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message/*"სერვერზე მოხდა შეცდომა."*/});
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