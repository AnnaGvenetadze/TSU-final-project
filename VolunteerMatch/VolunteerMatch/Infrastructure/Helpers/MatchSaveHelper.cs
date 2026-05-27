using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Infrastructure.Helpers
{
    // TODO: MatchSaveHelper დატესტე!!! (ეიაის დაბრუნებული იგივე წყვილის მეორე მხარიდან 
    // შენახვისას დუბლირებული წყვილის შენახვის მცდელობისას ცხრილი იუნიქ შეზღუდვა
    // გაბაგავს ინსერთს !
    /*
 1. Volunteer generate-მ უკვე შექმნა:
   VolunteerId = V
   EventId = E

2. Organization generate-მ იგივე Event E-ზე AI-სგან მიიღო იგივე Volunteer V

3. Organization service შექმნის VolunteerEventMatch object-ს იგივე წყვილით

4. MatchSaveHelper ნახავს, რომ ეს წყვილი უკვე არსებობს

5. ახალ duplicate-ს აღარ ჩასვამს

6. unique constraint error აღარ მოხდება
     */
    public class MatchSaveHelper
    {
        private readonly VolunteerMatchingDbContext _context;

        public MatchSaveHelper(VolunteerMatchingDbContext context)
        {
            _context = context;
        }


        public async Task<List<VolunteerEventMatch>> SaveOnlyNewMatchesAsync(
            List<VolunteerEventMatch> matches,
            CancellationToken cancellationToken = default)
        {
            if (matches.Count == 0)
            {
                return [];
            }

            var existingMatchKeys = await GetExistingMatchKeysAsync(
                matches,
                cancellationToken);

            var newMatches = GetNewMatches(
                matches,
                existingMatchKeys);

            if (newMatches.Count == 0)
            {
                return [];
            }

            _context.VolunteerEventMatches.AddRange(newMatches);
            await _context.SaveChangesAsync(cancellationToken);

            return newMatches;
        }



        private async Task<HashSet<(Guid VolunteerId, Guid EventId)>>
            GetExistingMatchKeysAsync(
                List<VolunteerEventMatch> matches,
                CancellationToken cancellationToken)
        {
            var volunteerIds = new HashSet<Guid>();
            var eventIds = new HashSet<Guid>();

            foreach (var match in matches)
            {
                volunteerIds.Add(match.VolunteerId);
                eventIds.Add(match.EventId);
            }

            var existingMatches = await _context.VolunteerEventMatches
                .AsNoTracking()
                .Where(match =>
                    volunteerIds.Contains(match.VolunteerId) &&
                    eventIds.Contains(match.EventId))
                .Select(match => new
                {
                    match.VolunteerId,
                    match.EventId
                })
                .ToListAsync(cancellationToken);

            return existingMatches
                .Select(match => (match.VolunteerId, match.EventId))
                .ToHashSet();
        }



        private static List<VolunteerEventMatch> GetNewMatches(
            List<VolunteerEventMatch> matches,
            HashSet<(Guid VolunteerId, Guid EventId)> existingMatchKeys)
        {
            return matches
                .Where(match => !existingMatchKeys.Contains(
                    (match.VolunteerId, match.EventId)))
                .ToList();
        }
    }
}