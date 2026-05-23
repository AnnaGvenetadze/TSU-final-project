using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;

namespace VolunteerMatch.Application.Services
{
    public class MyVolunteerMatchingService : IVolunteerMatchingService
    {
        private const int CandidateEventsLimit = 20;

        private readonly VolunteerMatchingDbContext _context;
        private readonly IAiMatchingClient _aiMatchingClient;

        public MyVolunteerMatchingService(
            VolunteerMatchingDbContext context,
            IAiMatchingClient aiMatchingClient)
        {
            _context = context;
            _aiMatchingClient = aiMatchingClient;
        }

        public async Task GenerateMyMatchesAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            var (volunteer, volunteerTagIds) = 
                await GetVolunteerMatchingInfoAsync(
                    volunteerId,cancellationToken);

            var candidateEvents = 
                await GetCandidateEventsAsync(
                    volunteerId, volunteerTagIds, cancellationToken);

            if (!candidateEvents.Any())
                return;
            
            var aiRequest = CreateAiBatchRequest(volunteer, candidateEvents);
            var aiResponse = 
                await _aiMatchingClient.GetMatchedEventIdsForVolunteerAsync(
                    aiRequest,
                    cancellationToken);

            var matchedEvents = candidateEvents
                .Where(e => aiResponse.MatchedEventIds.Contains(e.EventId))
                .ToList();

            var matches = matchedEvents
                .Select(e => CreateRecommendedMatch(volunteerId, e))
                .ToList();

            _context.VolunteerEventMatches.AddRange(matches);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<(VolunteerProfile Volunteer, List<Guid> TagIds)> 
            GetVolunteerMatchingInfoAsync(Guid volunteerId, CancellationToken cancellationToken)
        {
            var volunteer = await _context.VolunteerProfiles
                .Include(v => v.VolunteerTags)
                .FirstOrDefaultAsync(
                    v => v.VolunteerId == volunteerId,
                    cancellationToken);

            volunteer = Guard.EnsureFound(volunteer);

            var tagIds = volunteer.VolunteerTags
                .Select(vt => vt.TagId)
                .ToList();

            if (!tagIds.Any())
            {
                throw new ArgumentException("მეჩინგისთვის ჯერ აირჩიეთ ინტერესები.");
            }

            return (volunteer, tagIds);
        }

        private async Task<List<Event>> GetCandidateEventsAsync(
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

        private static AiBatchRequestDto CreateAiBatchRequest(
            VolunteerProfile volunteer,
            List<Event> candidateEvents)
        {
            return new AiBatchRequestDto
            {
                Volunteer = new AiVolunteerInfoDto
                {
                    Skills = volunteer.Skills,
                    Interests = volunteer.Interests
                },

                Events = candidateEvents
                    .Select(CreateAiEventInfo)
                    .ToList()
            };
        }

        private static AiEventInfoDto CreateAiEventInfo(Event eventItem)
        {
            var orderedTags = eventItem.EventTags
                .OrderBy(et => et.SortOrder)
                .Select(et => et.Tag.Name)
                .ToList();

            return new AiEventInfoDto
            {
                EventId = eventItem.EventId,
                Requirements = eventItem.Requirements,
                MainTheme = orderedTags.FirstOrDefault() ?? string.Empty,
                Tags = orderedTags
            };
        }

        private static VolunteerEventMatch CreateRecommendedMatch(
            Guid volunteerId,
            Event eventItem)
        {
            return new VolunteerEventMatch
            {
                VolunteerId = volunteerId,
                EventId = eventItem.EventId,
                RequestedByRole = UserRoles.Volunteer,
                Status = MatchStatus.Recommended
            };
        }
    }
}