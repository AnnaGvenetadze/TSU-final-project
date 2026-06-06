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
        private readonly VolunteerMatchingDbContext _context;
        private readonly IAiMatchingClient _aiMatchingClient;
        private readonly IAiMatchingLogger _aiMatchingLogger;
        private readonly IMapper _mapper;
        private readonly MatchingQueryHelper _matchingQueryHelper;
        private readonly FavoritesHelper _favoritesHelper;
        private readonly MatchSaveHelper _matchSaveHelper;
        private readonly MatchCleanupHelper _matchCleanupHelper;

        public MyVolunteerMatchingService(
            VolunteerMatchingDbContext context,
            IAiMatchingClient aiMatchingClient,
            IAiMatchingLogger aiMatchingLogger,
            IMapper mapper,
            MatchingQueryHelper matchingQueryHelper,
            FavoritesHelper favoritesHelper,
            MatchSaveHelper matchSaveHelper,
            MatchCleanupHelper matchCleanupHelper)
        {
            _context = context;
            _aiMatchingClient = aiMatchingClient;
            _aiMatchingLogger = aiMatchingLogger;
            _mapper = mapper;
            _matchingQueryHelper = matchingQueryHelper;
            _favoritesHelper = favoritesHelper;
            _matchSaveHelper = matchSaveHelper;
            _matchCleanupHelper = matchCleanupHelper;
        }



        public async Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid volunteerId,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var (volunteer, volunteerTagIds) =
                await _matchingQueryHelper.GetVolunteerMatchingInfoAsync(
                    volunteerId,
                    cancellationToken);

            var candidateEvents =
                await _matchingQueryHelper.GetCandidateEventsForVolunteerAsync(
                    volunteerId,
                    volunteerTagIds,
                    cancellationToken);

            _aiMatchingLogger.LogPrefilteredData(
                volunteer,
                candidateEvents);

            if (candidateEvents.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ამ ეტაპზე თქვენს ინტერესებზე მორგებული აქტიური ღონისძიებები ვერ მოიძებნა."
                };
            }

            var matchedEvents = await GetAiMatchedEventsAsync(
                volunteer,
                candidateEvents,
                cancellationToken);

            if (matchedEvents.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ხელოვნურმა ინტელექტმა შესაბამისი ღონისძიებები ვერ შეარჩია."
                };
            }

            var matches = VolunteerMatchingFactory.CreateRecommendedMatches(
                volunteerId,
                matchedEvents);
            var savedMatches = await _matchSaveHelper.SaveOnlyNewMatchesAsync(
                matches,
                cancellationToken);

            if (savedMatches.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ახალი რეკომენდებული ღონისძიებები ვერ მოიძებნა."
                };
            }

            return new CreateMatchesResultDto
            {
                CreatedMatchesCount = savedMatches.Count,
                Message = $"{savedMatches.Count} რეკომენდებული ღონისძიება წარმატებით მოიძებნა."
            };
        }



        public async Task<PagedResultDto<GetMatchedEventCardDto>> GetMyMatchesAsync(
            Guid currentUserId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                    cancellationToken);

            PaginationValidator.Validate(page, pageSize);
            Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .AnyAsync(
                        volunteer => volunteer.VolunteerId == currentUserId,
                        cancellationToken)
            );

            var query = _matchingQueryHelper.GetRecommendedEventMatchesQuery(
                currentUserId);

            var totalCount = await query.CountAsync(cancellationToken);

            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMatchedEventCardDto>>(matches);

            await _favoritesHelper.SetFavoriteMatchedEventsAsync(
                currentUserId,
                items,
                cancellationToken);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }



        public async Task RequestMyMatchAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.VolunteerId == volunteerId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Recommended)
            {
                throw new ArgumentException("მოთხოვნის გაგზავნა შესაძლებელია მხოლოდ რეკომენდებულ ღონისძიებაზე.");
            }

            match.Status = MatchStatus.Pending;
            match.RequestedByRole = UserRoles.Volunteer;
            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task RejectMyMatchAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.VolunteerId == volunteerId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Recommended)
            {
                throw new ArgumentException("უარყოფა შესაძლებელია მხოლოდ რეკომენდებული ღონისძიების.");
            }

            match.Status = MatchStatus.Rejected;
            match.RequestedByRole = UserRoles.Volunteer;
            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task<PagedResultDto<GetMatchedEventCardDto>> 
            GetMyMatchRequestsAsync(
                Guid volunteerId,
                int page,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            PaginationValidator.Validate(page, pageSize);
            Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .AnyAsync(
                        volunteer => volunteer.VolunteerId == volunteerId,
                        cancellationToken)
            );

            var query = _matchingQueryHelper.GetMyMatchRequestsQuery(
                volunteerId);

            var totalCount = await query.CountAsync(cancellationToken);
            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMatchedEventCardDto>>(matches);

            await _favoritesHelper.SetFavoriteMatchedEventsAsync(
                volunteerId,
                items,
                cancellationToken);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }



        private async Task<List<Event>> GetAiMatchedEventsAsync(
            VolunteerProfile volunteer,
            List<Event> candidateEvents,
            CancellationToken cancellationToken)
        {
            var aiRequest = VolunteerMatchingFactory.CreateAiBatchRequest(
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
                .Where(eventItem => matchedEventIds.Contains(eventItem.EventId))
                .ToList();
        }



        public async Task AcceptOrganizationMatchRequestAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToOrganizationMatchRequestAsync(
                volunteerId,
                matchId,
                MatchStatus.Accepted,
                cancellationToken);
        }



        public async Task DeclineOrganizationMatchRequestAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToOrganizationMatchRequestAsync(
                volunteerId,
                matchId,
                MatchStatus.Rejected,
                cancellationToken);
        }



        private async Task RespondToOrganizationMatchRequestAsync(
            Guid volunteerId,
            Guid matchId,
            MatchStatus newStatus,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .Include(match => match.Event)
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.VolunteerId == volunteerId,
                    cancellationToken);

            match = Guard.EnsureFound(match);

            if (match.Status != MatchStatus.Pending ||
                match.RequestedByRole != UserRoles.Organization)
            {
                throw new ArgumentException(
                    "მოქმედება შესაძლებელია მხოლოდ ორგანიზაციისგან შემოსულ შეთავაზებაზე.");
            }

            match.Status = newStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}