using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Services;


namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/volunteer")]
    public class VolunteerController : ControllerBase
    {
        private readonly VolunteerService _volunteerService;

        public VolunteerController(VolunteerService volunteerService)
        {
            _volunteerService = volunteerService
                ?? throw new ArgumentNullException(nameof(volunteerService));
        }

        [HttpGet("{volunteerId:guid}/profile")]
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
    }
}