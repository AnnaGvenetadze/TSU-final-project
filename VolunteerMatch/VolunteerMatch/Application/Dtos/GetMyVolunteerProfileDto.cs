namespace VolunteerMatch.Application.Dtos
{
    // TODO: თეგების, ფავორიტების და მეჩინგ საჯეშენების ლისტი (ნოთიფიკაციებში)?
    public class GetMyVolunteerProfileDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required DateOnly BirthDate { get; set; }

        public required string Citizenship { get; set; }

        public required string Profession { get; set; }

        public required string Languages { get; set; }

        public required string Skills { get; set; }

        public required string Interests { get; set; }

        public string? ProfilePhotoUrl { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? Education { get; set; }

        public string? Technologies { get; set; }

        public string? Experience { get; set; }

        public string? Description { get; set; }

        public required List<Guid> VolunteerTagIds { get; set; } = new();
    }
}
