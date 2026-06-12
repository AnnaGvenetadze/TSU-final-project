using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/volunteers/profile-options")]
    public class VolunteerProfileOptionsController : ControllerBase
    {
        private readonly IVolunteerProfileOptionsService _volunteerProfileOptionsService;

        public VolunteerProfileOptionsController(
            IVolunteerProfileOptionsService volunteerProfileOptionsService)
        {
            _volunteerProfileOptionsService = volunteerProfileOptionsService;
        }

        [HttpGet]
        public async Task<ActionResult<GetVolunteerProfileOptionsDto>> GetVolunteerProfileOptions(
            CancellationToken cancellationToken)
        {
            var options = await _volunteerProfileOptionsService
                .GetVolunteerProfileOptionsAsync(cancellationToken);

            return Ok(options);
        }
    }
}