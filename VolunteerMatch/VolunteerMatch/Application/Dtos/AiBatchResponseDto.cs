namespace VolunteerMatch.Application.Dtos
{
    public class AiBatchResponseDto
    {
        public List<Guid> MatchedEventIds { get; set; } = new();
    }
}