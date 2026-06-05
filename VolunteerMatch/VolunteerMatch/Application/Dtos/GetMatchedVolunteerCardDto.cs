namespace VolunteerMatch.Application.Dtos
{
    public class GetMatchedVolunteerCardDto
    {
        public required Guid VolunteerEventMatchId { get; set; }

        public required SearchVolunteerItemDto Volunteer { get; set; }
    }
}