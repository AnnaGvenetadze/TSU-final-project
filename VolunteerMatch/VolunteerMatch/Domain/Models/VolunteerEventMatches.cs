using VolunteerMatch.Domain.Constants;

namespace VolunteerMatch.Domain.Models
{
    public class VolunteerEventMatch
    {
        public Guid VolunteerEventMatchId { get; set; }
        public Guid VolunteerId { get; set; }
        public Guid EventId { get; set; }
        public string RequestedByRole { get; set; } = null!;
        public MatchStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public VolunteerProfile Volunteer { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}