using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(Notification notification);

        Task<List<GetNotificationDto>> GetMyNotificationsAsync(Guid userId);

        Task RemoveExpiredNotificationsAsync();
    }
}