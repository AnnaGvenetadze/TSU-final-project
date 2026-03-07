namespace VolunteerMatch.Models;
// TODO: ლოსტები განკომენტარდება
public partial class VolunteerProfile
{
    public Guid VolunteerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Citizenship { get; set; } = null!;

    //public string? Education { get; set; }

    public string Profession { get; set; } = null!;

    public string Languages { get; set; } = null!;

    public string Skills { get; set; } = null!;

    public string Interests { get; set; } = null!;

    public string? ProfilePhotoUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? Technologies { get; set; }

    public string? Experience { get; set; }

    public string? Description { get; set; }

    public virtual User Volunteer { get; set; } = null!;

    //public virtual ICollection<FavoriteEvent> FavoriteEvents { get; set; } = new List<FavoriteEvent>();

    //public virtual ICollection<MatchingSuggestion> MatchingSuggestions { get; set; } = new List<MatchingSuggestion>();

    //public virtual ICollection<VolunteerTag> VolunteerTags { get; set; } = new List<VolunteerTag>();
}
