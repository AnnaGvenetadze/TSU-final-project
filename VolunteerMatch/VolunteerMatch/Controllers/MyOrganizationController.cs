using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerMatch.Services;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/organization/profile")]
    [Authorize(Roles = "ორგანიზაცია")]
    public class MyOrganizationController : ControllerBase
    {
        private readonly MyOrganizationService _organizationService;

        public MyOrganizationController(MyOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var organizationId = GetCurrentUserId();

                var profile = await _organizationService.GetMyProfileAsync(organizationId);

                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("მომხმარებლის იდენტიფიკაცია ვერ მოხერხდა.");

            return userId;
        }
    }
}
