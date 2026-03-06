namespace VolunteerMatch.Dtos
{
    public class GetOrganizationProfileDto
    {
        // TODO: ივენთებიც უნდა გავატანო პროფილის ენფოინთში ან ცალკე ენდფოინთად
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LinkedInUrl { get; set; }
        public string Description { get; set; } = string.Empty;
        //public List<OrganizationProfileEventDto> Events { get; set; } = new();
    }
}
