namespace VolunteerMatch.Application.Dtos.Matching
{
    public class GetMatchedEventCardDto
    {
        public Guid VolunteerEventMatchId { get; set; }

        public bool IsFavorite { get; set; }

        public required GetEventCardDto Event { get; set; }
    }
}