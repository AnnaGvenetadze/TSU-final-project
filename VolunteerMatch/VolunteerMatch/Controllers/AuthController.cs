using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> RegisterVolunteer([FromBody] RegisterVolunteerDto dto)
        {   //400 - Bad Request (model validation failed)
            try
            {
                await _userService.RegisterVolunteerAsync(dto);
                return Ok("მოხალისე წარმატებით დარეგისტრირდა.");
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


        [HttpPost("register/organization")]
        public async Task<IActionResult> RegisterOrganization([FromBody] RegisterOrganizationDto dto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                await _userService.RegisterOrganizationAsync(dto);
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
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                var token = await _userService.AuthenticateUserAsync(loginUserDto);
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


        //[Authorize(Roles = "ორგანიზაცია")]
        //[HttpPost("eventcreation")]
        //public async Task<IActionResult> CreateEvent()
        //{
        //    return Ok("ივენთი წარმატებით შეიქმნა");
        //}
    }
}
