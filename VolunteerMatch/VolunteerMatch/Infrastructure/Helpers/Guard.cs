using System.Runtime.CompilerServices;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class Guard
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnsureFound<T>(T? entity) where T : class
        {
            if (entity is null)
                throw new KeyNotFoundException("ვერ მოიძებნა!");

            return entity;
        }
    }
}