namespace VolunteerMatch.Domain.Models;

public partial class Event
{
    public Guid EventId { get; set; }

    public Guid OrganizationId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Requirements { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTimeOffset StartDate { get; set; }

    public DateTimeOffset EndDate { get; set; }

    public TimeOnly DailyStartTime { get; set; }

    public TimeOnly DailyEndTime { get; set; }

    public int VolunteersAmount { get; set; }

    public string Benefits { get; set; } = null!;

    //public string? SpeakersJsons { get; set; } // დასამატებელია ან ცალკე 1 თეიბლად

    public string? MainPhotoUrl { get; set; }

    public string? Photo2Url { get; set; }

    public string? Photo3Url { get; set; }

    public string? AdditionalInfo { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual OrganizationProfile Organization { get; set; } = null!;

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();

    public virtual ICollection<FavoriteEvent> FavoriteEvents { get; set; } = new List<FavoriteEvent>();

    public ICollection<VolunteerEventMatch> VolunteerEventMatches { get; set; } = [];
}
