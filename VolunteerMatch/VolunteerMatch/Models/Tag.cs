namespace VolunteerMatch.Models;

public partial class Tag
{
    public Guid TagId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<VolunteerTag> VolunteerTags { get; set; } = new List<VolunteerTag>();
}
