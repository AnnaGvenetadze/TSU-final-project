using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Services;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/volunteers/me")]
    [Authorize(Roles = UserRoles.Volunteer)]
    public class MyVolunteerController : BaseController
    {
        private readonly MyVolunteerService _myVolunteerService;

        public MyVolunteerController(MyVolunteerService myVolunteerService)
        {
            _myVolunteerService = myVolunteerService
                ?? throw new ArgumentNullException(nameof(myVolunteerService));
        }


        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var profile = await _myVolunteerService.GetMyProfileAsync(CurrentUserId);

                return Ok(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateVolunteerProfileDto updateDto)
        {
            try
            {
                await _myVolunteerService.UpdateMyProfileAsync(CurrentUserId, updateDto);

                return Ok("მოხალისის პროფილი წარმატებით განახლდა.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
