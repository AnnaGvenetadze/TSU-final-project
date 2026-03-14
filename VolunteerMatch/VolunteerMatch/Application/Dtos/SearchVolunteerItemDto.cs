namespace VolunteerMatch.Application.Dtos
{
    public class SearchVolunteerItemDto
    {
        public required Guid VolunteerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required DateOnly DateOfBirth { get; set; }
        public required string Citizenship { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Description { get; set; }
        public string? Languages { get; set; }
    }
}