using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Constants;


namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/volunteers")]
    public class VolunteerController : ControllerBase // public endpoints-ებისთვის
    {
        private readonly VolunteerService _volunteerService;

        public VolunteerController(VolunteerService volunteerService)
        {
            _volunteerService = volunteerService
                ?? throw new ArgumentNullException(nameof(volunteerService));
        }

        [HttpGet("{volunteerId:guid}")]
        [Authorize(Roles = UserRoles.Organization)] // 403 - Forbidden
        public async Task<IActionResult> GetVolunteerProfile(Guid volunteerId)
        {
            try
            {
                var profile = await _volunteerService.GetVolunteerProfileAsync(volunteerId);

                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }


        [HttpGet("search")]
        [Authorize(Roles = UserRoles.Organization)]
        public async Task<ActionResult<List<SearchVolunteerItemDto>>> SearchVolunteers(
            [FromQuery] string searchTerm,
            [FromQuery] int take = 24)
        {
            try
            {
                var volunteers = await _volunteerService.SearchVolunteersAsync(searchTerm, take);

                return Ok(volunteers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა");
            }
        }
    }
}