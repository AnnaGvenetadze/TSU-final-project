// Private endpoints for creating, updating, and listing events created by the organization
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
                //return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }


        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetMyEventById(Guid eventId)
        {
            try
            {
                var eventDetails = await _myOrganizationEventsService
                    .GetMyEventByIdAsync(CurrentUserId, eventId);

                return Ok(eventDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
                //return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }


        [HttpPut("{eventId:guid}")]
        public async Task<IActionResult> UpdateMyEvent(Guid eventId, UpdateEventDetailsDto updateDto)
        {
            try
            {
                await _myOrganizationEventsService
                    .UpdateMyEventAsync(CurrentUserId, eventId, updateDto);

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


        [HttpDelete("{eventId:guid}")]
        public async Task<IActionResult> DeleteMyEvent(Guid eventId)
        {
            try
            {
                await _myOrganizationEventsService.DeleteMyEventAsync(CurrentUserId, eventId);

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
