namespace VolunteerMatch.Domain.Models
{
    public class Skill
    {
        public Guid SkillId { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<VolunteerSkill> VolunteerSkills { get; set; } = [];
    }
}