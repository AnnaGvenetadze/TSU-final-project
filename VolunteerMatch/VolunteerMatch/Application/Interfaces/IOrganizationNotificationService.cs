using VolunteerMatch.Application.Dtos.Notifications;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IOrganizationNotificationService
    {
        Task<GetMyOrgNotificationsDto> GetMyNotificationsAsync(
            Guid organizationId,
            int incomingPage,
            int acceptedPage,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}