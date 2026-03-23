namespace VolunteerMatch.Application.Dtos
{// TODO: Uncomment themes and map it with tag name in auto mapper
    public class GetVolunteerFavEventCardDto
    {
        public Guid EventId { get; set; }

        public bool IsActive { get; set; }

        public required string Title { get; set; }

        public required string OrganizationName { get; set; }

        public required string ShortDescription { get; set; }

        public required string Location { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public bool IsFavorite { get; set; }

        //public required string Theme { get; set; }

        public string? MainPhotoUrl { get; set; }
    }
}
