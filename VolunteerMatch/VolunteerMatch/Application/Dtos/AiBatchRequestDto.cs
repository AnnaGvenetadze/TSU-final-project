namespace VolunteerMatch.Application.Dtos
{
    public class AiBatchRequestDto
    {
        public required AiVolunteerInfoDto Volunteer { get; set; }

        public List<AiEventInfoDto> Events { get; set; } = new();
    }
}