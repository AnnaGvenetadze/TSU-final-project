using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class Tag
{
    public Guid TagId { get; set; }

    public string Name { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<VolunteerTag> VolunteerTags { get; set; } = new List<VolunteerTag>();
}
