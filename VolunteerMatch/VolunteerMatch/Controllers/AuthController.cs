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
        public async Task<IActionResult> RegisterVolunteer(RegisterVolunteerDto dto)
        {
            try
            {
                await _userService.RegisterVolunteerAsync(dto);
                return Ok("მოხალისე წარმატებით დარეგისტრირდა.");
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);    // 409
            }
        }


        [HttpPost("register/organization")]
        public async Task<IActionResult> RegisterOrganization(RegisterOrganizationDto dto)
        {
            try
            {
                await _userService.RegisterOrganizationAsync(dto);
                return Ok("ორგანიზაცია წარმატებით დარეგისტრირდა.");
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);    // 409
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            try
            {
                var token = await _userService.AuthenticateUserAsync(loginUserDto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "არასწორი იმეილი ან პაროლი." }); // 401
            }
        }
    }
}
