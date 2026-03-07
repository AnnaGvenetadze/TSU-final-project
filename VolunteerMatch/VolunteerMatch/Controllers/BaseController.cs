using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace VolunteerMatch.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected Guid CurrentUserId => GetCurrentUserId();

        protected string? CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value;

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("მომხმარებლის იდენტიფიკაცია ვერ მოხერხდა.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("მომხმარებლის ID არასწორია.");

            return userId;
        }
    }
}