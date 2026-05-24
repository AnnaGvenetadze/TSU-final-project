using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/volunteers/me/matches")]
    [Authorize(Roles = UserRoles.Volunteer)]
    public class MyVolunteerMatchingController : BaseController
    {
        private readonly IVolunteerMatchingService _matchingService;

        public MyVolunteerMatchingController(IVolunteerMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMyMatches(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _matchingService.GenerateMyMatchesAsync(
                    CurrentUserId,
                    cancellationToken);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (AiMatchingException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }
    }
}