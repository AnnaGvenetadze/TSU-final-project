namespace VolunteerMatch.Application.Dtos
{
    public class GetOrganizationProfileDto
    {
        public Guid OrganizationId { get; set; }
        public required string OrganizationName { get; set; }
        public required string Email { get; set; }
        public required string Description { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? ProfilePhotoUrl { get; set; } // დაემატა
    }
}
