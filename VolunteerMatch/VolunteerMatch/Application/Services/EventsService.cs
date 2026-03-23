using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Application.Dtos;
using AutoMapper.QueryableExtensions;

namespace VolunteerMatch.Application.Services
{
    public class EventsService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public EventsService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<PagedResultDto<GetEventCardDto>> GetEventsAsync(int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);

            var query = _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                .Where(eventModel => eventModel.IsActive)
                .OrderByDescending(eventModel => eventModel.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var events = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetEventCardDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return PaginationHelper
                        .CreatePagedResult(
                            _mapper.Map<List<GetEventCardDto>>(events),
                            page,
                            pageSize,
                            totalCount
                        );
        }


        public async Task<GetEventDetailsDto> GetEventByIdAsync(Guid eventId)
        {
            var eventEntity = Guard.EnsureFound(
                await _context.Events
                    .AsNoTracking()
                    .Include(eventModel => eventModel.Organization)
                    .Include(e => e.EventTags)
                    .SingleOrDefaultAsync(eventModel =>
                        eventModel.EventId == eventId &&
                        eventModel.IsActive)
            );

            return _mapper.Map<GetEventDetailsDto>(eventEntity);
        }
    }
}