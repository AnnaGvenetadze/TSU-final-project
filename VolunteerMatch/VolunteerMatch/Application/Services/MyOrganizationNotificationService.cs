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
    public class MyOrganizationNotificationService : IOrganizationNotificationService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;
        private readonly MatchCleanupHelper _matchCleanupHelper;

        public MyOrganizationNotificationService(
            VolunteerMatchingDbContext context,
            IMapper mapper,
            MatchCleanupHelper matchCleanupHelper)
        {
            _context = context;
            _mapper = mapper;
            _matchCleanupHelper = matchCleanupHelper;
        }


        public async Task<GetMyOrgNotificationsDto> GetMyNotificationsAsync(
            Guid organizationId,
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
                await _context.OrganizationProfiles
                    .AsNoTracking()
                    .AnyAsync(
                        organization => organization.OrganizationId == organizationId,
                        cancellationToken)
            );

            var incoming = await GetIncomingNotificationsAsync(
                organizationId,
                incomingPage,
                pageSize,
                cancellationToken);

            var accepted = await GetAcceptedNotificationsAsync(
                organizationId,
                acceptedPage,
                pageSize,
                cancellationToken);

            return new GetMyOrgNotificationsDto
            {
                Incoming = incoming,
                Accepted = accepted
            };
        }



        private async Task<PagedResultDto<GetMyOrgNotificationDto>>
            GetIncomingNotificationsAsync(
                Guid organizationId,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var query = GetBaseOrganizationMatchesQuery(organizationId)
                .Where(match => match.Status == MatchStatus.Pending)
                .Where(match => match.RequestedByRole == UserRoles.Volunteer)
                .OrderByDescending(match => match.CreatedAt);

            return await CreatePagedNotificationsAsync(
                query,
                page,
                pageSize,
                cancellationToken);
        }



        private async Task<PagedResultDto<GetMyOrgNotificationDto>>
            GetAcceptedNotificationsAsync(
                Guid organizationId,
                int page,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var query = GetBaseOrganizationMatchesQuery(organizationId)
                .Where(match => match.Status == MatchStatus.Accepted)
                .OrderByDescending(match => match.CreatedAt);

            return await CreatePagedNotificationsAsync(
                query,
                page,
                pageSize,
                cancellationToken);
        }



        private IQueryable<VolunteerEventMatch> GetBaseOrganizationMatchesQuery(
            Guid organizationId)
        {
            return _context.VolunteerEventMatches
                .AsNoTracking()
                .Include(match => match.Volunteer)
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.Organization)
                .Include(match => match.Event)
                    .ThenInclude(eventModel => eventModel.EventTags)
                        .ThenInclude(eventTag => eventTag.Tag)
                .Where(match => match.Event.OrganizationId == organizationId)
                .Where(match => match.Event.IsActive)
                .Where(match => match.Event.EndDate >= DateTimeOffset.UtcNow);
        }



        private async Task<PagedResultDto<GetMyOrgNotificationDto>>
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

            var items = _mapper.Map<List<GetMyOrgNotificationDto>>(matches);

            return PaginationHelper.CreatePagedResult(
                items,
                page,
                pageSize,
                totalCount);
        }
    }
}