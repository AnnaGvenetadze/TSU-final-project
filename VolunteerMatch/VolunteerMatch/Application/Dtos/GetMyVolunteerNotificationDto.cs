using VolunteerMatch.Application.Dtos.Matching;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Application.Dtos.Notifications
{
    public class GetMyVolunteerNotificationDto
    {
        public Guid VolunteerEventMatchId { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; } = null!;

        public string Message { get; set; } = null!;

        public MatchStatus Status { get; set; }

        public string RequestedByRole { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public GetEventCardDto Event { get; set; } = null!;
    }
}