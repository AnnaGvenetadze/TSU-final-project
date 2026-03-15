// TODO: Add endpoints for listing events created by a specific organization and getting details of a specific event created by the organization
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;

namespace VolunteerMatch.Presentation.Controllers
{
    [Route("api/organizations/{organizationId:guid}/events")]
    [ApiController]
    public class OrganizationEventsController : ControllerBase
    {
        private readonly OrganizationEventsService _organizationEventsService;

        public OrganizationEventsController(OrganizationEventsService organizationEventsService)
        {
            _organizationEventsService = organizationEventsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventsByOrganizationId(
            Guid organizationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 9)
        {
            try
            {
                var result = await _organizationEventsService
                    .GetEventsByOrganizationIdAsync(organizationId, page, pageSize);

                return Ok(result);
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
    }
}
