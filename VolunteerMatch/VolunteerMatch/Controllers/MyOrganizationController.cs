using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Dtos;
using VolunteerMatch.Services;
using VolunteerMatch.Constants;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api/organizations/me")]
    [Authorize(Roles = UserRoles.Organization)]
    public class MyOrganizationController : BaseController // for private endpoints
    {
        private readonly MyOrganizationService _myOrganizationService;

        public MyOrganizationController(MyOrganizationService myOrganizationService)
        {
            _myOrganizationService = myOrganizationService ?? throw new ArgumentNullException(nameof(myOrganizationService));
        }


        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var profile = await _myOrganizationService.GetMyProfileAsync(CurrentUserId);

                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile(UpdateOrganizationProfileDto updateDto)
        {
            try
            {
                await _myOrganizationService.UpdateMyProfileAsync(CurrentUserId, updateDto);

                return Ok("ორგანიზაციის პროფილი წარმატებით განახლდა.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch(ArgumentException ex)
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
