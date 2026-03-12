namespace VolunteerMatch.Dtos
{   // იმის მიხედვით რა ფროფერთების კონვერტაცია დაგვჭირდება
    // OrganizationProfile <-> GetOrganizationProfileDto
    // შეიცვლება OrganizationProfileMapping.cs
    public class GetOrganizationProfileDto
    {
        public required Guid OrganizationId { get; set; }
        public required string OrganizationName { get; set; }
        public required string Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? ProfilePhotoUrl { get; set; } // დაემატა
        public required string Description { get; set; }
    }
}
