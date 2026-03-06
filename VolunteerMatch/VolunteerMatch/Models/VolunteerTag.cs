namespace VolunteerMatch.Models;

public partial class VolunteerTag
{
    public Guid VolunteerId { get; set; }

    public Guid TagId { get; set; }

    public virtual Tag Tag { get; set; } = null!;

    public virtual VolunteerProfile Volunteer { get; set; } = null!;
}
