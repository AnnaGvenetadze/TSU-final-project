namespace VolunteerMatch.Domain.Models
{
    public class VolunteerInterest
    {
        public Guid VolunteerId { get; set; }

        public VolunteerProfile Volunteer { get; set; } = null!;

        public Guid InterestId { get; set; }

        public Interest Interest { get; set; } = null!;
    }
}