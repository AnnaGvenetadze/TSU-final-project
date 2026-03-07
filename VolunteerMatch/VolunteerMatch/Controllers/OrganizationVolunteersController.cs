using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Constants;
using VolunteerMatch.Dtos;
using VolunteerMatch.Services;
// private endpoint ორგანიზაციისთვის მოხალისეებთან ურთიერთქმედებისთვის,
// როგორიცაა ძებნა, სიების ნახვა და ა.შ.
    
namespace VolunteerMatch.Controllers
{
    [Route("api/organization/volunteers")]
    [ApiController]
    [Authorize(Roles = UserRoles.Organization)] // 403 - Forbidden
    public class OrganizationVolunteersController : ControllerBase
    {
        private readonly VolunteerService _volunteerService;

        public OrganizationVolunteersController(VolunteerService volunteerService)
        {
            _volunteerService = volunteerService
                ?? throw new ArgumentNullException(nameof(volunteerService));
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<SearchVolunteerItemDto>>> SearchVolunteers(
            [FromQuery] string searchTerm,
            [FromQuery] int take = 24)
        {
            try
            {
                var volunteers = await _volunteerService.SearchVolunteersAsync(searchTerm, take);

                return Ok(volunteers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "სერვერზე მოხდა შეცდომა");
            }
        }
    }
}
