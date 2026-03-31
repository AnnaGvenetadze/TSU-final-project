using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Application.Services;


namespace VolunteerMatch.Presentation.Controllers
{
    [Route("api/volunteers/me/favorites")]
    [ApiController]
    [Authorize(Roles = UserRoles.Volunteer)]
    public class FavoritesController : BaseController
    {
        private readonly FavoriteEventService _favoriteEventService;

        public FavoritesController(FavoriteEventService favoriteEventService)
        {
            _favoriteEventService = favoriteEventService
                ?? throw new ArgumentNullException(nameof(favoriteEventService));
        }


        [HttpPost("{eventId:guid}")]
        public async Task<IActionResult> AddFavorite(Guid eventId)
        {
            try
            {
                await _favoriteEventService.AddFavoriteAsync(CurrentUserId, eventId);

                return Ok(new { message = "ივენთი წარმატებით დაემატა ფავორიტებში." });
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

        
        [HttpGet]
        public async Task<IActionResult> GetMyFavoriteEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                var events = await _favoriteEventService.GetMyFavoriteEventsAsync(CurrentUserId, page, pageSize);

                return Ok(events);
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


        [HttpDelete("{eventId:guid}")]
        public async Task<IActionResult> DeleteFavorite(Guid eventId)
        {
            try
            {
                await _favoriteEventService.DeleteFavoriteAsync(CurrentUserId, eventId);

                return Ok(new { message = "ივენთი წაიშალა ფავორიტებიდან." });
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
        
    }
}
