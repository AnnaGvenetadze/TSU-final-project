using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class MatchingSuggestion
{
    public Guid SuggestionId { get; set; }

    public Guid VolunteerId { get; set; }

    public Guid EventId { get; set; }

    public string Initiator { get; set; } = null!;

    public byte MatchScore { get; set; }

    public bool? VolunteerApproved { get; set; }

    public bool? OrganizationApproved { get; set; }

    public string Status { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual VolunteerProfile Volunteer { get; set; } = null!;
}
