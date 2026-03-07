using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerMatch.Dtos;
using VolunteerMatch.Services;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/organization/me")]
    [Authorize(Roles = "ორგანიზაცია")]
    public class MyOrganizationController : ControllerBase // for private endpoints
    {
        private readonly MyOrganizationService _myOrganizationService;

        public MyOrganizationController(MyOrganizationService organizationService)
        {
            _myOrganizationService = organizationService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var organizationId = GetCurrentUserId();

                var profile = await _myOrganizationService.GetMyProfileAsync(organizationId);

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

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateMyProfile(UpdateOrganizationProfileDto updateDto)
        {
            var organizationId = GetCurrentUserId();

            await _myOrganizationService.UpdateMyProfileAsync(organizationId, updateDto);

            return NoContent();
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
