using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationController : ControllerBase // public endpoints-ებისთვის
    {
        private readonly OrganizationService _organizationService;

        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("{organizationId:guid}")]
        [Authorize(Roles = UserRoles.Volunteer)]
        public async Task<IActionResult> GetProfileById(Guid organizationId)
        {
            try
            {
                var profile = await _organizationService.GetProfileByIdAsync(organizationId);

                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }
    }
}
