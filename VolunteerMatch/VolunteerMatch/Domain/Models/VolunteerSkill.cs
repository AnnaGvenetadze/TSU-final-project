namespace VolunteerMatch.Domain.Models
{
    public class VolunteerSkill
    {
        public Guid VolunteerId { get; set; }

        public VolunteerProfile Volunteer { get; set; } = null!;

        public Guid SkillId { get; set; }

        public Skill Skill { get; set; } = null!;
    }
}