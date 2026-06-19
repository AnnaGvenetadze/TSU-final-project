using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/organizations/me/events/{eventId}/matches")]
    [Authorize(Roles = UserRoles.Organization)]
    public class MyOrganizationMatchingController : BaseController
    {
        private readonly IOrganizationMatchingService _matchingService;

        public MyOrganizationMatchingController(
            IOrganizationMatchingService matchingService)
        {
            _matchingService = matchingService;
        }



        [HttpPost("/api/organizations/me/matches/{matchId:guid}/accept")]
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



        [HttpPost("/api/organizations/me/matches/{matchId:guid}/decline")]
        public async Task<IActionResult> DeclineVolunteerMatchRequest(
            Guid matchId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _matchingService.DeclineVolunteerMatchRequestAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new { message = "მოხალისის მოთხოვნა უარყოფილია." });
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


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMyMatches(
                   Guid eventId,
                   CancellationToken cancellationToken)
        {
            try
            {
                var result = await _matchingService.GenerateMyMatchesAsync(
                    CurrentUserId,
                    eventId,
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



        [HttpGet]
        public async Task<IActionResult> GetMyMatches(
            Guid eventId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _matchingService.GetMyMatchesAsync(
                    CurrentUserId,
                    eventId,
                    page,
                    pageSize,
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
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message});//"სერვერზე მოხდა შეცდომა." });
            }
        }



        [HttpPost("{matchId:guid}/request")]
        public async Task<IActionResult> RequestMyMatch(
            Guid matchId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _matchingService.RequestMyMatchAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new { message = "მოხალისეს მოთხოვნა წარმატებით გაეგზავნა." });
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



        [HttpPost("{matchId:guid}/reject")]
        public async Task<IActionResult> RejectMyMatch(
           Guid matchId,
           CancellationToken cancellationToken)
        {
            try
            {
                await _matchingService.RejectMyMatchAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new { message = "რეკომენდებული მოხალისე უარყოფილია." });
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