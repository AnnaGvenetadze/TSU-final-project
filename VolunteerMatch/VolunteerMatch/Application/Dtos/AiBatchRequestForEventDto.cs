namespace VolunteerMatch.Application.Dtos
{
    public class AiBatchRequestForEventDto
    {
        public required AiEventInfoDto Event { get; set; }

        public List<AiVolunteerInfoForEventDto> Volunteers { get; set; } = new();
    }
}