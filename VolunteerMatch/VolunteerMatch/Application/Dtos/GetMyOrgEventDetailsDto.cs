namespace VolunteerMatch.Application.Dtos
{// TODO: ConfirmedVolunteersAmount
 // TODO: სპიკერები დაემატება
    public class GetMyOrgEventDetailsDto
    {
        public Guid EventId { get; set; }

        public required string Title { get; set; }

        public required string OrganizationName { get; set; }

        public bool IsActive { get; set; }

        public required string Email { get; set; }

        public required string Description { get; set; }

        public required string Requirements { get; set; }

        public required string Location { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public TimeOnly DailyStartTime { get; set; }

        public TimeOnly DailyEndTime { get; set; }

        public int VolunteersAmount { get; set; }

        // public int ConfirmedVolunteersAmount { get; set; }

        public required string Benefits { get; set; }

        public string? MainPhotoUrl { get; set; }

        public string? Photo2Url { get; set; }

        public string? Photo3Url { get; set; }

        public string? AdditionalInfo { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public required List<Guid> EventTagIds { get; set; } = new();
    }
}
