using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Services;


namespace VolunteerMatch.Controllers
{
    [Route("api/organization")] // TODO: ორგანიზაციის დასერჩვა სახელის ან სხვა პარამეტრის მიხედვით ენფოინთ(ებ)ი
    [ApiController]
    public class OrganizationController : ControllerBase // for public endpoints
    {
        private readonly OrganizationService _organizationService;

        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet("{organizationId:guid}/profile")]
        public async Task<IActionResult> GetProfileById(Guid organizationId)
        {
            try
            {
                var profile = await _organizationService.GetProfileByIdAsync(organizationId);

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
    }
}
