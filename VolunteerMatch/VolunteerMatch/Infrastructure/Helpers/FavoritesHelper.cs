using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos.Matching;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public class FavoritesHelper
    {
        private readonly VolunteerMatchingDbContext _context;

        public FavoritesHelper(VolunteerMatchingDbContext context)
        {
            _context = context;
        }

        public async Task SetFavoriteMatchedEventsAsync(
            Guid volunteerId,
            List<GetMatchedEventCardDto> matchedEvents,
            CancellationToken cancellationToken = default)
        {
            if (matchedEvents.Count == 0)
            {
                return;
            }

            var eventIds = matchedEvents
                .Select(matchedEvent => matchedEvent.Event.EventId)
                .ToList();

            var favoriteEventIds = await _context.FavoriteEvents
                .AsNoTracking()
                .Where(favorite => favorite.VolunteerId == volunteerId)
                .Where(favorite => eventIds.Contains(favorite.EventId))
                .Select(favorite => favorite.EventId)
                .ToListAsync(cancellationToken);

            var favoriteEventIdsSet = favoriteEventIds.ToHashSet();

            foreach (var matchedEvent in matchedEvents)
            {
                matchedEvent.IsFavorite =
                    favoriteEventIdsSet.Contains(matchedEvent.Event.EventId);
            }
        }
    }
}