namespace VolunteerMatch.Domain.Models
{
    public class Interest
    {
        public Guid InterestId { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<VolunteerInterest> VolunteerInterests { get; set; } = [];
    }
}