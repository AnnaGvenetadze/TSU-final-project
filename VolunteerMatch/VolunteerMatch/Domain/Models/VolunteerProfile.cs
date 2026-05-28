namespace VolunteerMatch.Domain.Models;
// TODO: ლისტები განკომენტარდება
public partial class VolunteerProfile
{
    public Guid VolunteerId { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required DateOnly BirthDate { get; set; }

    public required string Citizenship { get; set; }

    public required string Profession { get; set; }

    public required string Languages { get; set; }

    public required string Skills { get; set; }

    public required string Interests { get; set; }

    public string? Education { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? Technologies { get; set; }

    public string? Experience { get; set; }

    public string? Description { get; set; }

    public virtual User Volunteer { get; set; } = null!;

    public virtual ICollection<FavoriteEvent> FavoriteEvents { get; set; } = new List<FavoriteEvent>();

    public virtual ICollection<VolunteerTag> VolunteerTags { get; set; } = new List<VolunteerTag>();
}
