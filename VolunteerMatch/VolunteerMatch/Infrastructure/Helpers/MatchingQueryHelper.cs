using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public class MatchingQueryHelper
    {
        private const int CandidateEventsLimit = 20;

        private readonly VolunteerMatchingDbContext _context;

        public MatchingQueryHelper(VolunteerMatchingDbContext context)
        {
            _context = context;
        }


        public async Task<(VolunteerProfile Volunteer, List<Guid> TagIds)>
            GetVolunteerMatchingInfoAsync(
                Guid volunteerId,
                CancellationToken cancellationToken)
        {
            var volunteer = await _context.VolunteerProfiles
                .Include(v => v.VolunteerTags)
                    .ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync(
                    v => v.VolunteerId == volunteerId,
                    cancellationToken);

            volunteer = Guard.EnsureFound(volunteer);

            var tagIds = volunteer.VolunteerTags
                .Select(vt => vt.TagId)
                .ToList();

            if (tagIds.Count == 0)
            {
                throw new ArgumentException("მეჩინგისთვის ჯერ აირჩიეთ ინტერესები.");
            }

            return (volunteer, tagIds);
        }



        public async Task<List<Event>> GetCandidateEventsForVolunteerAsync(
            Guid volunteerId,
            List<Guid> volunteerTagIds,
            CancellationToken cancellationToken)
        {
            return await _context.Events
                .Include(e => e.EventTags)
                    .ThenInclude(et => et.Tag)
                .Where(e => e.IsActive)
                .Where(e => e.EndDate >= DateTimeOffset.UtcNow)
                .Where(e => e.EventTags.Any(et => volunteerTagIds.Contains(et.TagId)))
                .Where(e => !_context.VolunteerEventMatches
                    .Any(m => m.VolunteerId == volunteerId && m.EventId == e.EventId))
                .OrderByDescending(e => e.CreatedAt)
                .Take(CandidateEventsLimit)
                .ToListAsync(cancellationToken);
        }



        public IQueryable<VolunteerEventMatch> GetRecommendedEventMatchesQuery(
            Guid volunteerId)
        {
            return _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.Organization)
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.EventTags)
                        .ThenInclude(eventTag => eventTag.Tag)
                .Where(match => match.VolunteerId == volunteerId)
                .Where(match => match.Status == MatchStatus.Recommended)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow)
                .OrderByDescending(match => match.CreatedAt);
        }



        public IQueryable<VolunteerEventMatch> GetMyMatchRequestsQuery(Guid volunteerId)
        {
            return _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.Organization)
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.EventTags)
                        .ThenInclude(eventTag => eventTag.Tag)
                .Where(match => match.VolunteerId == volunteerId)
                .Where(match => match.Status == MatchStatus.Pending)
                .Where(match => match.RequestedByRole == UserRoles.Volunteer)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow)
                .OrderByDescending(match => match.CreatedAt);
        }
    }
}