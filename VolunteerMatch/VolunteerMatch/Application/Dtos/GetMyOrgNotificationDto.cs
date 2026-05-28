using VolunteerMatch.Application.Dtos.Matching;
using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Application.Dtos.Notifications
{
    public class GetMyOrgNotificationDto
    {
        public Guid VolunteerEventMatchId { get; set; }

        public Guid VolunteerId { get; set; }

        public string VolunteerFullName { get; set; } = null!;

        public string Message { get; set; } = null!;

        public MatchStatus Status { get; set; }

        public string RequestedByRole { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public GetEventCardDto Event { get; set; } = null!;
    }
}