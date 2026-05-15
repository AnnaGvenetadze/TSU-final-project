namespace VolunteerMatch.Application.Dtos
{
    public class AiEventInfoDto
    {
        public required Guid EventId { get; set; }
        public required string Requirements { get; set; }
        public required string MainTheme { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}
