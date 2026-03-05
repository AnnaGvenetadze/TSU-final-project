using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class OrganizationComment
{
    public Guid CommentId { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid AuthorVolunteerId { get; set; }

    public string Text { get; set; } = null!;

    public byte StarRating { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual VolunteerProfile AuthorVolunteer { get; set; } = null!;

    public virtual OrganizationProfile Organization { get; set; } = null!;
}
