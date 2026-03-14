namespace VolunteerMatch.Application.Dtos
{
    public class AuthResponseDto
    {
        public required string AccessToken { get; set; }
        public Guid UserId { get; set; }
        public required string Role { get; set; }
        public string Message { get; set; } = null!;
    }
}