using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Notifications;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;

namespace VolunteerMatch.Application.Services
{
    public class MyVolunteerNotificationService : IVolunteerNotificationService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;
        private readonly MatchCleanupHelper _matchCleanupHelper;

        public MyVolunteerNotificationService(
            VolunteerMatchingDbContext context,
            IMapper mapper,
            MatchCleanupHelper matchCleanupHelper)
        {
            _context = context;
            _mapper = mapper;
            _matchCleanupHelper = matchCleanupHelper;
        }


        public async Task<GetMyVolunteerNotificationsDto> GetMyNotificationsAsync(
            Guid volunteerId,
            int incomingPage,
            int acceptedPage,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await _matchCleanupHelper.DeleteInactiveOrExpiredMatchesAsync(
                cancellationToken);

            PaginationValidator.Validate(incomingPage, pageSize);
            PaginationValidator.Validate(acceptedPage, pageSize);

            Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .AnyAsync(
                        volunteer => volunteer.VolunteerId == volunteerId,
                        cancellationToken)
            );

            var incoming = await GetIncomingNotificationsAsync(
                volunteerId,
                incomingPage,
                pageSize,
                cancellationToken);

            var accepted = await GetAcceptedNotificationsAsync(
                volunteerId,
                acceptedPage,
                pageSize,
                cancellationToken);

            return new GetMyVolunteerNotificationsDto
            {
                Incoming = incoming,
                Accepted = accepted
            };
        }



        private async Task<PagedResultDto<GetMyVolunteerNotificationDto>>
            GetIncomingNotificationsAsync(
                Guid volunteerId,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var query = GetBaseVolunteerMatchesQuery(volunteerId)
                .Where(match => match.Status == MatchStatus.Pending)
                .Where(match => match.RequestedByRole == UserRoles.Organization)
                .OrderByDescending(match => match.CreatedAt);

            return await CreatePagedNotificationsAsync(
                query,
                page,
                pageSize,
                cancellationToken);
        }



        private async Task<PagedResultDto<GetMyVolunteerNotificationDto>>
            GetAcceptedNotificationsAsync(
                Guid volunteerId,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var query = GetBaseVolunteerMatchesQuery(volunteerId)
                .Where(match => match.Status == MatchStatus.Accepted)
                .OrderByDescending(match => match.CreatedAt);

            return await CreatePagedNotificationsAsync(
                query,
                page,
                pageSize,
                cancellationToken);
        }



        private IQueryable<VolunteerEventMatch> GetBaseVolunteerMatchesQuery(
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
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow);
        }



        private async Task<PagedResultDto<GetMyVolunteerNotificationDto>>
            CreatePagedNotificationsAsync(
                IQueryable<VolunteerEventMatch> query,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var matches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = _mapper.Map<List<GetMyVolunteerNotificationDto>>(matches);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }
    }
}