using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Dtos;
using VolunteerMatch.Services;
using VolunteerMatch.Constants;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/volunteer/me/profile")]
    [Authorize(Roles = UserRoles.Volunteer)]
    public class MyVolunteerProfileController : BaseController
    {
        private readonly MyVolunteerService _myVolunteerService;

        public MyVolunteerProfileController(MyVolunteerService myVolunteerService)
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
            }
        }
    }
}
