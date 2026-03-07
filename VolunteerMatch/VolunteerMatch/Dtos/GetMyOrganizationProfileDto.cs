namespace VolunteerMatch.Dtos
{
    // TODO: ივენთებიც უნდა გავატანო პროფილის ენფოინთში ან ცალკე ენდფოინთად
    public class GetMyOrganizationProfileDto
    {
        public required string OrganizationName { get; set; }
        public required string Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? ProfilePhotoUrl { get; set; } // დაემატა
        public required string Description { get; set; }
        //public List<OrganizationProfileEventDto> Events { get; set; } = new();
    }
}
