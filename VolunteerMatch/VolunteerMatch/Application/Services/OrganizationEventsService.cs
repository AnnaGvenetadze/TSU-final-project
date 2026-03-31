using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Domain.Models;

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
            Guid organizationId, int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);
            Guard.EnsureFound(
                await _context.OrganizationProfiles
                    .AnyAsync(o => o.OrganizationId == organizationId)
            );

            var query = GetActiveEventsQuery(organizationId);
            var totalCount = await query.CountAsync();

            var events = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetEventCardDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return PaginationHelper.CreatePagedResult(events, page, pageSize, totalCount);
        }


        public async Task<PagedResultDto<GetVolunteerEventCardDto>> GetEventsForVolunteerByOrganizationIdAsync(
            Guid organizationId, Guid volunteerId, int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);

            Guard.EnsureFound(
                await _context.OrganizationProfiles
                    .AnyAsync(o => o.OrganizationId == organizationId)
            );

            var query = GetActiveEventsQuery(organizationId);
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

        
        private IOrderedQueryable<Event> GetActiveEventsQuery(Guid organizationId)
        {
            return _context.Events
                .AsNoTracking()
                .Where(e => e.OrganizationId == organizationId && e.IsActive)
                .OrderByDescending(e => e.CreatedAt);
        }
    }
}