using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerMatch.Application.Interfaces;


namespace VolunteerMatch.Presentation.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly INotificationService _INotificationService;

    public NotificationsController(INotificationService INotificationService)
    {
        _INotificationService = INotificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        try
        {
            var notifications = await _INotificationService.GetMyNotificationsAsync(CurrentUserId);
            return Ok(notifications);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "ავტორიზაცია საჭიროა." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "სერვერზე მოხდა შეცდომა." });
        }
    }
}