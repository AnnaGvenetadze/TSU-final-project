using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Application.Dtos;
using AutoMapper.QueryableExtensions;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Application.Interfaces;
using System.Threading;

namespace VolunteerMatch.Application.Services
{
    public class EventsService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEventCapacityService _eventCapacityService;

        public EventsService(VolunteerMatchingDbContext context, IMapper mapper, IEventCapacityService eventCapacityService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventCapacityService = eventCapacityService 
                ?? throw new ArgumentNullException(nameof(eventCapacityService));
        }


        public async Task<PagedResultDto<GetEventCardDto>> GetEventsForOrganizationAsync(int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);

            var query = GetActiveEventsQuery();
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

        public async Task<PagedResultDto<GetVolunteerEventCardDto>> GetEventsForVolunteerAsync(
            Guid volunteerId, int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);

            var query = GetActiveEventsQuery();
            var totalCount = await query.CountAsync();

            var events = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(eventModel => new
                {
                    Event = eventModel,
                    IsFavorite = eventModel.FavoriteEvents
                        .Any(favorite => favorite.VolunteerId == volunteerId)
                })
                .ToListAsync();

            var items = events.Select(x =>
            {
                var dto = _mapper.Map<GetVolunteerEventCardDto>(x.Event);
                dto.IsFavorite = x.IsFavorite;
                return dto;
            }).ToList();

            return PaginationHelper.CreatePagedResult(items, page, pageSize, totalCount);
        }

        public async Task<GetEventDetailsDto> GetEventByIdAsync(Guid eventId)
        {
            var eventEntity = Guard.EnsureFound(
                await _context.Events
                    .AsNoTracking()
                    .Include(eventModel => eventModel.Organization)
                        .ThenInclude(organization => organization.Organization)
                    .Include(eventModel => eventModel.EventTags)
                    .SingleOrDefaultAsync(eventModel =>
                        eventModel.EventId == eventId &&
                        eventModel.IsActive)
            );

            var acceptedVolunteersCount = await _context.VolunteerEventMatches
                .AsNoTracking()
                .CountAsync(match =>
                    match.EventId == eventId &&
                    match.Status == MatchStatus.Accepted);

            var dto = _mapper.Map<GetEventDetailsDto>(eventEntity);
            dto.AcceptedVolunteersCount = await _eventCapacityService
                .GetAcceptedVolunteersCountAsync(eventId, default);

            dto.IsFilled = dto.AcceptedVolunteersCount >= dto.VolunteersAmount;

            return dto;
        }

        private IQueryable<Event> GetActiveEventsQuery()
        {
            return _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                .Where(eventModel => eventModel.IsActive)
                .OrderByDescending(eventModel => eventModel.CreatedAt);
        }
    }
}