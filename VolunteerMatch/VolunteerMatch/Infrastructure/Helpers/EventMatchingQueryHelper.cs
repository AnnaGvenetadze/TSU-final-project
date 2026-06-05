using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public class EventMatchingQueryHelper
    {
        private const int CandidateVolunteersLimit = 20;

        private readonly VolunteerMatchingDbContext _context;

        public EventMatchingQueryHelper(VolunteerMatchingDbContext context)
        {
            _context = context;
        }


        public async Task<(Event Event, List<Guid> TagIds)>
            GetEventMatchingInfoAsync(
                Guid organizationId,
                Guid eventId,
                CancellationToken cancellationToken)
        {
            var eventItem = await _context.Events
                .Include(eventModel => eventModel.EventTags)
                    .ThenInclude(eventTag => eventTag.Tag)
                .FirstOrDefaultAsync(
                    eventModel =>
                        eventModel.EventId == eventId &&
                        eventModel.OrganizationId == organizationId,
                    cancellationToken);

            eventItem = Guard.EnsureFound(eventItem);

            if (!eventItem.IsActive)
            {
                throw new ArgumentException("მეჩინგისთვის ღონისძიება აქტიური უნდა იყოს.");
            }

            if (eventItem.EndDate < DateTimeOffset.UtcNow)
            {
                throw new ArgumentException("ვადაგასულ ღონისძიებაზე მეჩინგი შეუძლებელია.");
            }

            var tagIds = eventItem.EventTags
                .Select(eventTag => eventTag.TagId)
                .ToList();

            if (tagIds.Count == 0)
            {
                throw new ArgumentException("მეჩინგისთვის ღონისძიებას უნდა ჰქონდეს თეგები.");
            }

            return (eventItem, tagIds);
        }



        public async Task<List<VolunteerProfile>> GetCandidateVolunteersForEventAsync(
            Guid eventId,
            List<Guid> eventTagIds,
            CancellationToken cancellationToken)
        {
            return await _context.VolunteerProfiles
                .Include(volunteer => volunteer.VolunteerTags)
                    .ThenInclude(volunteerTag => volunteerTag.Tag)
                .Where(volunteer => volunteer.VolunteerTags
                    .Any(volunteerTag => eventTagIds.Contains(volunteerTag.TagId)))
                .Where(volunteer => !_context.VolunteerEventMatches
                    .Any(match =>
                        match.VolunteerId == volunteer.VolunteerId &&
                        match.EventId == eventId))
                .OrderBy(volunteer => volunteer.LastName)
                .ThenBy(volunteer => volunteer.FirstName)
                .Take(CandidateVolunteersLimit)
                .ToListAsync(cancellationToken);
        }



        public IQueryable<VolunteerEventMatch> GetRecommendedVolunteerMatchesQuery(
            Guid organizationId,
            Guid eventId)
        {
            return _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Volunteer)
                    .ThenInclude(volunteer => volunteer.Volunteer)
                .Include(match => match.Event)
                .Where(match => match.EventId == eventId)
                .Where(match => match.Event.OrganizationId == organizationId)
                .Where(match => match.Status == MatchStatus.Recommended)
                .Where(match => match.RequestedByRole == UserRoles.Organization)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow)
                .OrderByDescending(match => match.CreatedAt);
        }



        public IQueryable<VolunteerEventMatch> GetMyMatchRequestsQuery(
            Guid organizationId,
            Guid eventId)
        {
            return _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Volunteer)
                    .ThenInclude(volunteer => volunteer.Volunteer)
                .Include(match => match.Event)
                .Where(match => match.EventId == eventId)
                .Where(match => match.Event.OrganizationId == organizationId)
                .Where(match => match.Status == MatchStatus.Pending)
                .Where(match => match.RequestedByRole == UserRoles.Organization)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow)
                .OrderByDescending(match => match.CreatedAt);
        }
    }
}