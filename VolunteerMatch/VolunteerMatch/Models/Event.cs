namespace VolunteerMatch.Models;

public partial class Event
{
    public Guid EventId { get; set; }

    public Guid OrganizationId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Requirements { get; set; } = null!;

    public byte? AgeMin { get; set; }

    public byte? AgeMax { get; set; }

    public string Location { get; set; } = null!;

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public string Status { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// ახალი დამატებული
    public string SpeakersJson { get; set; } = null!; // ჯსონის ობიექტების კოლექცია ერთ ველად

    public string Benefits { get; set; } = null!;

    public string? AdditionalInfo { get; set; }
    ///

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<FavoriteEvent> FavoriteEvents { get; set; } = new List<FavoriteEvent>();

    public virtual ICollection<MatchingSuggestion> MatchingSuggestions { get; set; } = new List<MatchingSuggestion>();

    public virtual OrganizationProfile Organization { get; set; } = null!;
}
