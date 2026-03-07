namespace VolunteerMatch.Dtos
{
    // TODO: ივენთებიც უნდა გავატანო პროფილის ენფოინთში ან ცალკე ენდფოინთად
    public class GetMyOrganizationProfileDto
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LinkedInUrl { get; set; }
        public string? ProfilePhotoUrl { get; set; } // დაემატა
        public string Description { get; set; } = string.Empty;
        //public List<OrganizationProfileEventDto> Events { get; set; } = new();
    }
}
