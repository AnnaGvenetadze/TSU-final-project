using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class EventCapacityService : IEventCapacityService
    {
        private readonly VolunteerMatchingDbContext _context;

        public EventCapacityService(VolunteerMatchingDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetAcceptedVolunteersCountAsync(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            return await _context.VolunteerEventMatches
                .AsNoTracking()
                .CountAsync(
                    match =>
                        match.EventId == eventId &&
                        match.Status == MatchStatus.Accepted,
                    cancellationToken);
        }

        public async Task<bool> IsFilledAsync(
            Guid eventId,
            int volunteersAmount,
            CancellationToken cancellationToken = default)
        {
            var acceptedCount = await GetAcceptedVolunteersCountAsync(
                eventId,
                cancellationToken);

            return acceptedCount >= volunteersAmount;
        }

        public async Task EnsureNotFilledAsync(
            Guid eventId,
            int volunteersAmount,
            CancellationToken cancellationToken = default)
        {
            var isFilled = await IsFilledAsync(
                eventId,
                volunteersAmount,
                cancellationToken);

            if (isFilled)
            {
                throw new ArgumentException("ღონისძიება უკვე შევსებულია.");
            }
        }
    }
}