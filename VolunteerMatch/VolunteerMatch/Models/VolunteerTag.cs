using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class VolunteerTag
{
    public Guid VolunteerId { get; set; }

    public Guid TagId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Tag Tag { get; set; } = null!;

    public virtual VolunteerProfile Volunteer { get; set; } = null!;
}
