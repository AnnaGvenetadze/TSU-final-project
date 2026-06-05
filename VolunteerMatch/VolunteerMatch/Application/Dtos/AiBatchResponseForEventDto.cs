namespace VolunteerMatch.Application.Dtos
{
    public class AiBatchResponseForEventDto
    {
        public List<Guid> MatchedVolunteerIds { get; set; } = new();
    }
}