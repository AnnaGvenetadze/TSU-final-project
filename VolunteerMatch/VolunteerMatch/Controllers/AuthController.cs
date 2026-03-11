using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Dtos;
using VolunteerMatch.Exceptions;
using VolunteerMatch.Services;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register/volunteer")]
        public async Task<IActionResult> RegisterVolunteer([FromBody] CreateVolunteerDto createDto)
        {   //400 - Bad Request (model validation failed)
            try
            {
                await _userService.RegisterVolunteerAsync(createDto);
                return Ok("მოხალისე წარმატებით დარეგისტრირდა.");
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(new { message = ex.Message });    // 409
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });  // 400
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }


        [HttpPost("register/organization")]
        public async Task<IActionResult> RegisterOrganization([FromBody] CreateOrganizationDto createDto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                await _userService.RegisterOrganizationAsync(createDto);
                return Ok("ორგანიზაცია წარმატებით დარეგისტრირდა.");
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);    // 409
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                var token = await _userService.AuthenticateUserAsync(loginDto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "არასწორი იმეილი ან პაროლი." }); // 401
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
            }
        }
    }
}
