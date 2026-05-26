using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Matching;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Ai;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;

namespace VolunteerMatch.Application.Services
{
    public class MyVolunteerMatchingService : IVolunteerMatchingService
    {
        private const int CandidateEventsLimit = 20;

        private readonly VolunteerMatchingDbContext _context;
        private readonly IAiMatchingClient _aiMatchingClient;
        private readonly IAiMatchingLogger _aiMatchingLogger;
        private readonly IMapper _mapper;

        public MyVolunteerMatchingService(
            VolunteerMatchingDbContext context,
            IAiMatchingClient aiMatchingClient,
            IAiMatchingLogger aiMatchinglogger,
            IMapper mapper)
        {
            _context = context;
            _aiMatchingClient = aiMatchingClient;
            _aiMatchingLogger = aiMatchinglogger;
            _mapper = mapper;
        }

        public async Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            var matchingData = await GetPrefilteredMatchingDataAsync(
                volunteerId,
                cancellationToken);

            // ტესტირებისთვის
            _aiMatchingLogger.LogPrefilteredData(
                matchingData.Volunteer,
                matchingData.CandidateEvents);

            Console.WriteLine("LogPrefilteredData finished");

            if (matchingData.CandidateEvents.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ამ ეტაპზე თქვენს ინტერესებზე მორგებული აქტიური ღონისძიებები ვერ მოიძებნა."
                };
            }

            var matchedEvents = await GetAiMatchedEventsAsync(
                matchingData.Volunteer,
                matchingData.CandidateEvents,
                cancellationToken);

            if (matchedEvents.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ხელოვნურმა ინტელექტმა შესაბამისი ღონისძიებები ვერ შეარჩია."
                };
            }

            var matches = CreateRecommendedMatches(
                volunteerId,
                matchedEvents);

            _context.VolunteerEventMatches.AddRange(matches);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateMatchesResultDto
            {
                CreatedMatchesCount = matches.Count,
                Message = $"{matches.Count} რეკომენდებული ღონისძიება წარმატებით მოიძებნა."
            };
        }

        public async Task<PagedResultDto<GetMatchedEventCardDto>> GetMyMatchesAsync(
            Guid currentUserId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            PaginationValidator.Validate(page, pageSize);

            var volunteerExists = await _context.VolunteerProfiles
                .AnyAsync(v => v.VolunteerId == currentUserId, cancellationToken);

            Guard.EnsureFound(volunteerExists);

            var query = _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.Organization)
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.EventTags)
                        .ThenInclude(eventTag => eventTag.Tag)
                .Where(match => match.VolunteerId == currentUserId)
                .Where(match => match.Status == MatchStatus.Recommended)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow)
                .OrderByDescending(match => match.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMatchedEventCardDto>>(matches);

            await SetFavoriteStatusesAsync(
                currentUserId,
                items,
                cancellationToken);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }

        private sealed record VolunteerMatchingData(
            VolunteerProfile Volunteer,
            List<Event> CandidateEvents);

        private async Task<VolunteerMatchingData> GetPrefilteredMatchingDataAsync(
            Guid volunteerId,
            CancellationToken cancellationToken)
        {
            var (volunteer, volunteerTagIds) =
                await GetVolunteerMatchingInfoAsync(
                    volunteerId,
                    cancellationToken);

            var candidateEvents =
                await GetCandidateEventsAsync(
                    volunteerId,
                    volunteerTagIds,
                    cancellationToken);

            return new VolunteerMatchingData(
                volunteer,
                candidateEvents);
        }

        private async Task<List<Event>> GetAiMatchedEventsAsync(
            VolunteerProfile volunteer,
            List<Event> candidateEvents,
            CancellationToken cancellationToken)
        {
            var aiRequest = CreateAiBatchRequest(
                volunteer,
                candidateEvents);

            _aiMatchingLogger.LogAiRequest(aiRequest);

            var aiResponse =
                await _aiMatchingClient.GetMatchedEventIdsForVolunteerAsync(
                    aiRequest,
                    cancellationToken);

            _aiMatchingLogger.LogAiResponse(aiResponse);

            var matchedEventIds = aiResponse.MatchedEventIds
                .ToHashSet();

            return candidateEvents
                .Where(e => matchedEventIds.Contains(e.EventId))
                .ToList();
        }
        
        private async Task SetFavoriteStatusesAsync(
            Guid volunteerId,
            List<GetMatchedEventCardDto> matchedEvents,
            CancellationToken cancellationToken)
        {
            if (matchedEvents.Count == 0)
            {
                return;
            }

            var eventIds = matchedEvents
                .Select(match => match.Event.EventId)
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

        private static List<VolunteerEventMatch> CreateRecommendedMatches(
            Guid volunteerId,
            List<Event> matchedEvents)
        {
            return matchedEvents
                .Select(e => CreateRecommendedMatch(volunteerId, e))
                .ToList();
        }

        private async Task<(VolunteerProfile Volunteer, List<Guid> TagIds)>
            GetVolunteerMatchingInfoAsync(Guid volunteerId, CancellationToken cancellationToken)
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
                /* იგივე ივენთები რომ არ გაიგზავნოს რომ დაბრუნებისას მეჩების 
                 * ცხრილში ბაზიდან უნიკალურმა ქონსთრეინთმა არ დაბაგოს ჩასმა */ 
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