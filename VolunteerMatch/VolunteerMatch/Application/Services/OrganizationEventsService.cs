using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;

namespace VolunteerMatch.Application.Services
{
    public class OrganizationEventsService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public OrganizationEventsService(
            VolunteerMatchingDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<PagedResultDto<GetEventCardDto>> GetEventsByOrganizationIdAsync(
            Guid organizationId,
            int page,
            int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);
            Guard.EnsureFound(
                await _context.OrganizationProfiles
                    .AnyAsync(o => o.OrganizationId == organizationId)
            );

            var query = _context.Events
                .AsNoTracking()
                .Where(e => e.OrganizationId == organizationId && e.IsActive)
                .OrderByDescending(e => e.CreatedAt);

            var totalCount = await query.CountAsync();

            var events = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetEventCardDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return PaginationHelper.CreatePagedResult(events, page, pageSize, totalCount);
        }
    }
}