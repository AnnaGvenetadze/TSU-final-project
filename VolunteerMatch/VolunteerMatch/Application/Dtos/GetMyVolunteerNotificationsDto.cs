using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Dtos.Notifications
{
    public class GetMyVolunteerNotificationsDto
    {
        public PagedResultDto<GetMyVolunteerNotificationDto> Incoming { get; set; } = null!;

        public PagedResultDto<GetMyVolunteerNotificationDto> Accepted { get; set; } = null!;
    }
}