namespace VolunteerMatch.Dtos
{   // TODO: იმის მიხედვით რა ფროფერთების კონვერტაცია დაგვჭირდება
    // OrganizationProfile <-> GetOrganizationProfileDto
    // შეიცვლება OrganizationProfileMapping.cs
    public class GetOrganizationProfileDto
    {
        public Guid OrganizationId { get; set; } // აქ ამის დაბრუნება რად მინდა ?
        public string OrganizationName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LinkedInUrl { get; set; } 
        public string? ProfilePhotoUrl { get; set; } // დაემატა
        public string Description { get; set; } = string.Empty;
        // public List<OrganizationProfileEventDto> Events { get; set; } = new();
    }
}
