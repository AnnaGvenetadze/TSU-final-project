using System.Security.Cryptography;
using System.Text;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class RefreshTokenHelper
    {
        public static string Generate()
        { 
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }


        public static string Hash(string refreshToken)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
            var hashBytes = SHA256.HashData(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }


        public static bool IsActive(RefreshToken refreshToken, DateTime now)
        {
            // გამოთხოვილი (გაუქმებული) ან ვადაგასული
            return refreshToken.RevokedAt is null &&
                   refreshToken.ExpiresAt > now;
        }


        public static RefreshToken CreateEntity(
            Guid userId, string tokenHash, DateTime expiresAt)
        {
            return new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                RevokedAt = null
            };
        }
    }
}