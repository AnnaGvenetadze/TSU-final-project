using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Presentation.Controllers
{
    [ApiController]
    [Route("api/organizations/me/notifications")]
    [Authorize(Roles = UserRoles.Organization)]
    public class MyOrganizationNotificationsController : BaseController
    {
        private readonly IOrganizationNotificationService _notificationService;

        public MyOrganizationNotificationsController(
            IOrganizationNotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] int incomingPage = 1,
            [FromQuery] int acceptedPage = 1,
            [FromQuery] int pageSize = 6,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _notificationService.GetMyNotificationsAsync(
                    CurrentUserId,
                    incomingPage,
                    acceptedPage,
                    pageSize,
                    cancellationToken);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "სერვერზე მოხდა შეცდომა.",
                    //error = ex.Message,
                    //innerError = ex.InnerException?.Message
                });
            }
        }
    }
}