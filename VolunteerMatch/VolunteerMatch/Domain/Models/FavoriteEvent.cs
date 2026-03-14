using System;
using System.Collections.Generic;

namespace VolunteerMatch.Domain.Models;

public partial class FavoriteEvent
{
    public Guid VolunteerId { get; set; }

    public Guid EventId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual VolunteerProfile Volunteer { get; set; } = null!;
}
