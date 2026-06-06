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



        [HttpGet]
        public async Task<IActionResult> GetMyMatches(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _matchingService.GetMyMatchesAsync(
                    CurrentUserId,
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
            catch
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
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

                return Ok(new { message = "ღონისძიებაზე მოთხოვნა წარმატებით გაიგზავნა." });
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

                return Ok(new { message = "რეკომენდებული ღონისძიება უარყოფილია." });
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



        [HttpPost("{matchId:guid}/accept")]
        public async Task<IActionResult> AcceptOrganizationMatchRequest(
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _matchingService.AcceptOrganizationMatchRequestAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new
                {
                    message = "ორგანიზაციის შეთავაზება წარმატებით დადასტურდა."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "სერვერზე მოხდა შეცდომა.",
                    //error = ex.Message,
                    //innerError = ex.InnerException?.Message
                });
            }
        }



        [HttpPost("{matchId:guid}/decline")]
        public async Task<IActionResult> DeclineOrganizationMatchRequest(
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _matchingService.DeclineOrganizationMatchRequestAsync(
                    CurrentUserId,
                    matchId,
                    cancellationToken);

                return Ok(new
                {
                    message = "ორგანიზაციის შეთავაზება უარყოფილია."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "სერვერზე მოხდა შეცდომა.",
                    //error = ex.Message,
                    //innerError = ex.InnerException?.Message
                });
            }
        }
        ///* თუ მოხალისის გაგზავნილი მეჩ რიქუესთების ფრონტზე ასახვა მოვისურვეთ */
        //[HttpGet("requests")]
        //public async Task<IActionResult> GetMySentMatchRequests(
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 6,
        //    CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var result = await _matchingService.GetMyMatchRequestsAsync(
        //            CurrentUserId,
        //            page,
        //            pageSize,
        //            cancellationToken);

        //        return Ok(result);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch
        //    {
        //        return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
        //    }
        //}
    }
}