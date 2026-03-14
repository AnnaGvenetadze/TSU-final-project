namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class Guard
    {
        public static T EnsureFound<T>(T? entity) where T : class
        {
            if (entity is null)
                throw new KeyNotFoundException("ვერ მოიძებნა!");

            return entity;
        }

        public static void EnsureFound(bool condition)
        {
            if (!condition)
                throw new KeyNotFoundException("ვერ მოიძებნა!");
        }
    }
}