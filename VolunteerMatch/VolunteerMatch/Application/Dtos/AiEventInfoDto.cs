namespace VolunteerMatch.Application.Dtos
{
    public class AiEventInfoDto
    {
        public required string Requirements { get; set; }
        public required string Theme { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
