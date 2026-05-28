using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Dtos.Notifications
{
    public class GetMyOrgNotificationsDto
    {
        public PagedResultDto<GetMyOrgNotificationDto> Incoming { get; set; } = null!;

        public PagedResultDto<GetMyOrgNotificationDto> Accepted { get; set; } = null!;
    }
}