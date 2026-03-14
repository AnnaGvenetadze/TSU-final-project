using System;
using System.Collections.Generic;

namespace VolunteerMatch.Domain.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public required string Email { get; set; }

    public string PasswordHash { get; set; } = null!;

    public required string Role { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual OrganizationProfile? OrganizationProfile { get; set; }

    public virtual VolunteerProfile? VolunteerProfile { get; set; }
}
