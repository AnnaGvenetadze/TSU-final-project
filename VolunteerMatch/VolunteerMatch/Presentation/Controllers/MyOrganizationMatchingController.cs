using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/organizations/me/matches")]
    [Authorize(Roles = UserRoles.Organization)]
    public class MyOrganizationMatchingController : BaseController
    {
        private readonly IOrganizationMatchingService _matchingService;

        public MyOrganizationMatchingController(
            IOrganizationMatchingService matchingService)
        {
            _matchingService = matchingService;
        }



        [HttpPost("{matchId:guid}/accept")]
        public async Task<IActionResult> AcceptVolunteerMatchRequest(
            Guid matchId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _matchingService.AcceptVolunteerMatchRequestAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new { message = "მოხალისის მოთხოვნა დადასტურებულია." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }
    }
}