using VolunteerMatch.Application.Dtos.Notifications;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerNotificationService
    {
        Task<GetMyVolunteerNotificationsDto> GetMyNotificationsAsync(
            Guid volunteerId,
            int incomingPage,
            int acceptedPage,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}