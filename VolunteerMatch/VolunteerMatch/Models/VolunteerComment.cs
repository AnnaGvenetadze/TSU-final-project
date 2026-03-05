using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class VolunteerComment
{
    public Guid CommentId { get; set; }

    public Guid VolunteerId { get; set; }

    public Guid AuthorOrganizationId { get; set; }

    public string Text { get; set; } = null!;

    public byte StarRating { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual OrganizationProfile AuthorOrganization { get; set; } = null!;

    public virtual VolunteerProfile Volunteer { get; set; } = null!;
}
