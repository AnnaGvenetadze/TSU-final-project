// Private endpoints for creating, updating, and listing events created by the organization
// TODO: GET api/organizations/me/matches/events
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Constants;
using VolunteerMatch.Dtos;
using VolunteerMatch.Services;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/organizations/me/events")]
    [Authorize(Roles = UserRoles.Organization)]
    public class MyOrganizationEventsController : BaseController
    {
        private readonly MyOrganizationEventsService _myOrganizationEventsService;

        public MyOrganizationEventsController(MyOrganizationEventsService myOrganizationEventsService)
        {
            _myOrganizationEventsService = myOrganizationEventsService 
                ?? throw new ArgumentNullException(nameof(myOrganizationEventsService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventDetailsDto createDto)
        {
            try
            {
                var eventId = await _myOrganizationEventsService
                    .CreateEventAsync(CurrentUserId, createDto);

                return Ok(new { eventId, MessageContent = "ივენთი წარმატებით დაემატა." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMyEventById(Guid id)
        {
            try
            {
                var eventDetails = await _myOrganizationEventsService
                    .GetMyEventByIdAsync(CurrentUserId, id);

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


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMyEvent(Guid id, UpdateEventDetailsDto updateDto)
        {
            try
            {
                await _myOrganizationEventsService
                    .UpdateMyEventAsync(CurrentUserId, id, updateDto);

                return Ok(new { message = "ივენთი წარმატებით განახლდა." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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


        [HttpGet]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var events = await _myOrganizationEventsService
                    .GetMyEventsAsync(CurrentUserId);

                return Ok(events);
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


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMyEvent(Guid id)
        {
            try
            {
                await _myOrganizationEventsService.DeleteMyEventAsync(CurrentUserId, id);

                return Ok("თქვენი ივენთი წარმატებით წაიშალა.");
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
