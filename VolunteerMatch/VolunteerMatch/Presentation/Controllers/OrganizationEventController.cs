// TODO: Add endpoints for listing events created by a specific organization and getting details of a specific event created by the organization
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [Route("api/organizations/{organizationId:guid}/events")]
    [ApiController]
    //[Authorize(Roles = $"{UserRoles.Volunteer},{UserRoles.Organization}")]
    public class OrganizationEventsController : BaseController
    {
        private readonly OrganizationEventsService _organizationEventsService;

        public OrganizationEventsController(OrganizationEventsService organizationEventsService)
        {
            _organizationEventsService = organizationEventsService;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Volunteer)]
        public async Task<IActionResult> GetEventsByOrganizationId(
            Guid organizationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 9)
        {
            try
            {
                if (CurrentUserRole == UserRoles.Volunteer)
                {
                    var volunteerEvents = await _organizationEventsService
                        .GetEventsForVolunteerByOrganizationIdAsync(
                            organizationId, 
                            CurrentUserId, 
                            page, 
                            pageSize
                        );

                    return Ok(volunteerEvents);
                }
                
                var organizationEvents = await _organizationEventsService
                    .GetEventsByOrganizationIdAsync(organizationId, page, pageSize);

                return Ok(organizationEvents);
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
