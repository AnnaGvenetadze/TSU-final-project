using System.Security.Cryptography;
using System.Text;

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
    }
}