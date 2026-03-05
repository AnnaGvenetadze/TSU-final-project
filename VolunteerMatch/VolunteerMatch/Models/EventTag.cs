using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class EventTag
{
    public Guid EventId { get; set; }

    public Guid TagId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
