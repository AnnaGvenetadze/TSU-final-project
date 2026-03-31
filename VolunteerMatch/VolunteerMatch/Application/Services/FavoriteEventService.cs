using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Validators;
using AutoMapper;


namespace VolunteerMatch.Application.Services
{
    public class FavoriteEventService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public FavoriteEventService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddFavoriteAsync(Guid volunteerId, Guid eventId)
        {
            Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .AnyAsync(v => v.VolunteerId == volunteerId)
            );
            Guard.EnsureFound(
                await _context.Events
                    .AsNoTracking()
                    .AnyAsync(e => e.EventId == eventId && e.IsActive)
            );

            var alreadyExists = await _context.FavoriteEvents
                .AsNoTracking()
                .AnyAsync(f => f.VolunteerId == volunteerId && f.EventId == eventId);

            if (alreadyExists)
                throw new ArgumentException("ივენთი უკვე დამატებულია ფავორიტებში.");

            var favorite = new FavoriteEvent
            {
                VolunteerId = volunteerId,
                EventId = eventId
            };

            _context.FavoriteEvents.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFavoriteAsync(Guid volunteerId, Guid eventId)
        {
            var favorite = await _context.FavoriteEvents
                .SingleOrDefaultAsync(f => f.VolunteerId == volunteerId && f.EventId == eventId);

            Guard.EnsureFound(favorite);

            _context.FavoriteEvents.Remove(favorite!);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResultDto<GetEventCardDto>> GetMyFavoriteEventsAsync(
            Guid volunteerId, int page, int pageSize)
        {
            PaginationValidator.Validate(page, pageSize);

            var query = GetFavoritesQuery(volunteerId);
            var totalCount = await query.CountAsync();

            var favorites = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(f => f.Event)
                    .ThenInclude(e => e.Organization)
                .Include(f => f.Event)
                    .ThenInclude(e => e.EventTags)
                        .ThenInclude(et => et.Tag)
                .ToListAsync();

            var items = favorites
                .Select(favorite => _mapper.Map<GetEventCardDto>(favorite.Event))
                .ToList();

            return PaginationHelper.CreatePagedResult(items, page, pageSize, totalCount);
        }

        private IOrderedQueryable<FavoriteEvent> GetFavoritesQuery(Guid volunteerId)
        {
            return _context.FavoriteEvents
                .AsNoTracking()
                .Where(favorite =>
                    favorite.VolunteerId == volunteerId &&
                    favorite.Event.IsActive)
                .OrderByDescending(favorite => favorite.CreatedAt);
        }
    }
}
