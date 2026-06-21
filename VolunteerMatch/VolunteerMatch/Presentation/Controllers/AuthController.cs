using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Application.Services;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }


        [HttpPost("register/volunteer")]
        public async Task<IActionResult> RegisterVolunteer([FromBody] CreateVolunteerDto createDto)
        {   //400 - Bad Request (model validation failed)
            try
            {
                var response = await _userService.RegisterVolunteerAsync(createDto);
                response.Message = "მოხალისე წარმატებით დარეგისტრირდა.";

                return Ok(response);
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(new { message = ex.Message });    // 409
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });  // 400
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }


        [HttpPost("register/organization")]
        public async Task<IActionResult> RegisterOrganization([FromBody] CreateOrganizationDto createDto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                var response = await _userService.RegisterOrganizationAsync(createDto);
                response.Message = "ორგანიზაცია წარმატებით დარეგისტრირდა.";

                return Ok(response);
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);    // 409
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });  // 400
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginDto)
        {   // 400 - Bad Request (model validation failed)
            try
            {
                var response = await _userService.AuthenticateUserAsync(loginDto);
                response.Message = "მომხმარებელი წარმატებით ავტორიზდა.";

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "არასწორი იმეილი ან პაროლი." }); // 401
            }
            catch (Exception ex)
            {
                //return StatusCode(500, "სერვერზე მოხდა შეცდომა.");
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}