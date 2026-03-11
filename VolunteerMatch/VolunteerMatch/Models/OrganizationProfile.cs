using System;
using System.Collections.Generic;

namespace VolunteerMatch.Models;

public partial class OrganizationProfile
{   // TODO: ივენთების ფუნქციონალის შექმნის მერე ივენთების ლისტიც უნდა დაემატოს 
    public Guid OrganizationId { get; set; }

    public required string OrganizationName { get; set; }

    public required string Description { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public virtual User Organization { get; set; } = null!;

    //public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
