namespace VolunteerMatch.Application.Dtos
{
    public class AuthResponseDto
    {
        public required string AccessToken { get; set; }
        // თუ ქუქი დაემატება რეფრეშ ტოკენი აღარ დაბრუნდება
        public required string RefreshToken { get; set; }
        public Guid UserId { get; set; }
        public required string Role { get; set; }
        public string Message { get; set; } = null!;
    }
}