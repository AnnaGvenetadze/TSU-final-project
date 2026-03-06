using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class OrganizationProfile
{
    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; } = null!;

    public string Description { get; set; } = null!;

    //public string? PhoneNumber { get; set; }

    public string? LinkedInUrl { get; set; }

    //public decimal AverageRating { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User Organization { get; set; } = null!;

    //public virtual ICollection<OrganizationComment> OrganizationComments { get; set; } = new List<OrganizationComment>();

    //public virtual ICollection<VolunteerComment> VolunteerComments { get; set; } = new List<VolunteerComment>();
}
