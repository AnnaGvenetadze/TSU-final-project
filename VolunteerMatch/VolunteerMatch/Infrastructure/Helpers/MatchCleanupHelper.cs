using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public class MatchCleanupHelper
    {
        private readonly VolunteerMatchingDbContext _context;

        public MatchCleanupHelper(VolunteerMatchingDbContext context)
        {
            _context = context;
        }

        public async Task DeleteMatchesForEventIdAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var matchesToDelete = await _context.VolunteerEventMatches
                .Where(match => match.EventId == eventId)
                .ToListAsync(cancellationToken);

            if (matchesToDelete.Count == 0)
            {
                return;
            }

            _context.VolunteerEventMatches.RemoveRange(matchesToDelete);
        }



        public async Task DeleteInactiveOrExpiredMatchesAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var matchesToDelete = await _context.VolunteerEventMatches
                .Where(match =>
                    !match.Event.IsActive ||
                    match.Event.EndDate < now)
                .ToListAsync(cancellationToken);

            if (matchesToDelete.Count == 0)
            {
                return;
            }

            _context.VolunteerEventMatches.RemoveRange(matchesToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}