namespace VolunteerMatch.Application.Dtos
{
    public class GetEventCardDto
    {
        public Guid EventId { get; set; }

        public required string Title { get; set; }

        public required string OrganizationName { get; set; }

        public required string ShortDescription { get; set; }

        public required string Location { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public required string Theme { get; set; }

        public string? MainPhotoUrl { get; set; }
    }
}
