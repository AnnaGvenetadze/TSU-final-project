using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;

namespace VolunteerMatch.Application.Services
{
    public class MyOrganizationMatchingService : IOrganizationMatchingService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IAiMatchingClient _aiMatchingClient;
        private readonly IMapper _mapper;
        private readonly EventMatchingQueryHelper _eventMatchingQueryHelper;
        private readonly MatchSaveHelper _matchSaveHelper;
        private readonly MatchCleanupHelper _matchCleanupHelper;
        private readonly IEventCapacityService _eventCapacityService;

        public MyOrganizationMatchingService(
            VolunteerMatchingDbContext context,
            IAiMatchingClient aiMatchingClient,
            IMapper mapper,
            EventMatchingQueryHelper eventMatchingQueryHelper,
            MatchSaveHelper matchSaveHelper,
            MatchCleanupHelper matchCleanupHelper, 
            IEventCapacityService eventCapacityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _aiMatchingClient = aiMatchingClient 
                ?? throw new ArgumentNullException(nameof(aiMatchingClient));
            _mapper = mapper;
            _eventMatchingQueryHelper = eventMatchingQueryHelper
                ?? throw new ArgumentNullException(nameof(eventMatchingQueryHelper));
            _matchSaveHelper = matchSaveHelper
                ?? throw new ArgumentNullException(nameof(matchSaveHelper));
            _matchCleanupHelper = matchCleanupHelper
                ?? throw new ArgumentNullException(nameof(matchCleanupHelper));
            _eventCapacityService = eventCapacityService
                ?? throw new ArgumentNullException(nameof(eventCapacityService));
        }


        public async Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
           Guid organizationId,
           Guid eventId,
           CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync();

            var (eventItem, eventTagIds) =
                await _eventMatchingQueryHelper.GetEventMatchingInfoAsync(
                    organizationId,
                    eventId,
                    cancellationToken);

            var candidateVolunteers =
                await _eventMatchingQueryHelper.GetCandidateVolunteersForEventAsync(
                    eventId,
                    eventTagIds,
                    cancellationToken);

            if (candidateVolunteers.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ამ ეტაპზე ამ ღონისძიებისთვის შესაბამისი ახალი მოხალისეები ვერ მოიძებნა."
                };
            }

            var matchedVolunteers = await GetAiMatchedVolunteersAsync(
                eventItem,
                candidateVolunteers,
                cancellationToken);

            if (matchedVolunteers.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ხელოვნურმა ინტელექტმა შესაბამისი მოხალისეები ვერ შეარჩია."
                };
            }

            var matches = EventMatchingFactory.CreateRecommendedMatches(
                eventId,
                matchedVolunteers);

            var savedMatches = await _matchSaveHelper.SaveOnlyNewMatchesAsync(
                matches,
                cancellationToken);

            if (savedMatches.Count == 0)
            {
                return new CreateMatchesResultDto
                {
                    CreatedMatchesCount = 0,
                    Message = "ახალი რეკომენდებული მოხალისეები ვერ მოიძებნა."
                };
            }

            return new CreateMatchesResultDto
            {
                CreatedMatchesCount = savedMatches.Count,
                Message = $"{savedMatches.Count} რეკომენდებული მოხალისე წარმატებით მოიძებნა."
            };
        }



        public async Task<PagedResultDto<GetMatchedVolunteerCardDto>>
            GetMyMatchesAsync(
                 Guid organizationId,
                 Guid eventId,
                 int page,
                 int pageSize,
                 CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            PaginationValidator.Validate(page, pageSize);

            var (eventItem, _) = await _eventMatchingQueryHelper.GetEventMatchingInfoAsync(
                organizationId,
                eventId,
                cancellationToken);

            var isFilled = await _eventCapacityService.IsFilledAsync(
                eventId,
                eventItem.VolunteersAmount,
                cancellationToken);

            if (isFilled)
            {
                return PaginationHelper.CreatePagedResult(
                    new List<GetMatchedVolunteerCardDto>(),
                    page,
                    pageSize,
                    0);
            }

            var query = _eventMatchingQueryHelper.GetRecommendedVolunteerMatchesQuery(
                organizationId,
                eventId);

            var totalCount = await query.CountAsync(cancellationToken);

            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMatchedVolunteerCardDto>>(matches);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }



        public async Task RequestMyMatchAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .Include(match => match.Event)
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.Event.OrganizationId == organizationId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Recommended)
            {
                throw new ArgumentException("მოთხოვნის გაგზავნა შესაძლებელია მხოლოდ რეკომენდებულ მოხალისეზე.");
            }
            await _eventCapacityService.EnsureNotFilledAsync(
                match.EventId,
                match.Event.VolunteersAmount,
                cancellationToken);

            match.Status = MatchStatus.Pending;
            match.RequestedByRole = UserRoles.Organization;
            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task RejectMyMatchAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            var match = await _context.VolunteerEventMatches
                .Include(match => match.Event)
                .FirstOrDefaultAsync(
                    match =>
                        match.VolunteerEventMatchId == matchId &&
                        match.Event.OrganizationId == organizationId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Recommended)
            {
                throw new ArgumentException("უარყოფა შესაძლებელია მხოლოდ რეკომენდებული მოხალისის.");
            }

            match.Status = MatchStatus.Rejected;
            match.RequestedByRole = UserRoles.Organization;
            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task<PagedResultDto<GetMatchedVolunteerCardDto>>
            GetMyMatchRequestsAsync(
                Guid organizationId,
                Guid eventId,
                int page,
                int pageSize,
                CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            PaginationValidator.Validate(page, pageSize);

            await _eventMatchingQueryHelper.GetEventMatchingInfoAsync(
                organizationId,
                eventId,
                cancellationToken);

            var query = _eventMatchingQueryHelper.GetMyMatchRequestsQuery(
                organizationId,
                eventId);

            var totalCount = await query.CountAsync(cancellationToken);

            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMatchedVolunteerCardDto>>(matches);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }



        public async Task AcceptVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToVolunteerMatchRequestAsync(
                organizationId,
                matchId,
                MatchStatus.Accepted,
                cancellationToken);
        }



        public async Task DeclineVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default)
        {
            await RespondToVolunteerMatchRequestAsync(
                organizationId,
                matchId,
                MatchStatus.Rejected,
                cancellationToken);
        }



        private async Task<List<VolunteerProfile>> GetAiMatchedVolunteersAsync(
            Event eventItem,
            List<VolunteerProfile> candidateVolunteers,
            CancellationToken cancellationToken)
        {
            var aiRequest = EventMatchingFactory.CreateAiBatchRequest(
                eventItem,
                candidateVolunteers);

            var aiResponse =
                await _aiMatchingClient.GetMatchedVolunteerIdsForEventAsync(
                    aiRequest,
                    cancellationToken);

            var matchedVolunteerIds = aiResponse.MatchedVolunteerIds
                .ToHashSet();

            return candidateVolunteers
                .Where(volunteer => matchedVolunteerIds.Contains(volunteer.VolunteerId))
                .ToList();
        }



        private async Task RespondToVolunteerMatchRequestAsync(
            Guid organizationId,
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
                        match.Event.OrganizationId == organizationId,
                    cancellationToken);

            match = Guard.EnsureFound(match);
            if (match.Status != MatchStatus.Pending ||
                match.RequestedByRole != UserRoles.Volunteer)
            {
                throw new ArgumentException("მოქმედება შესაძლებელია მხოლოდ მოხალისისგან შემოსულ მოთხოვნაზე.");
            }
            if (newStatus == MatchStatus.Accepted)
            {
                await _eventCapacityService.EnsureNotFilledAsync(
                    match.EventId,
                    match.Event.VolunteersAmount,
                    cancellationToken);
            }

            match.Status = newStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}