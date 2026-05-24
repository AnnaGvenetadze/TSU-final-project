namespace VolunteerMatch.Application.Dtos
{
    public class CreateMatchesResultDto
    {
        public int CreatedMatchesCount { get; set; }
        public required string Message { get; set; }
    }
}