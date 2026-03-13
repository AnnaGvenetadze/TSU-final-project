using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Validators;

namespace VolunteerMatch.Services
{
    public class MyOrganizationEventsService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public MyOrganizationEventsService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Guid> CreateEventAsync(Guid organizationId, CreateEventDetailsDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));
            EventValidator.ValidateForCreate(createDto);

            var organizationExists = await _context.OrganizationProfiles
                .AnyAsync(organizationProfile => 
                    organizationProfile.OrganizationId == organizationId);

            if (!organizationExists)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            var newEvent = _mapper.Map<Event>(createDto);
            newEvent.OrganizationId = organizationId;

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return newEvent.EventId;
        }


        public async Task<GetEventDetailsDto> GetMyEventByIdAsync(Guid organizationId, Guid eventId)
        {
            var eventEntity = Guard.EnsureFound(
                await _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                .ThenInclude(organizationProfile =>
                        organizationProfile.Organization)
                .SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive)
                );

            return _mapper.Map<GetEventDetailsDto>(eventEntity);
        }

        public async Task UpdateMyEventAsync(Guid organizationId, Guid eventId, UpdateEventDetailsDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            EventValidator.ValidateForUpdate(updateDto);

            var eventEntity = Guard.EnsureFound(
                await _context.Events.SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive)
                );

            _mapper.Map(updateDto, eventEntity);

            await _context.SaveChangesAsync();
        }



        public async Task<List<GetEventCardDto>> GetMyEventsAsync(Guid organizationId)
        {
            var organizationExists = await _context.OrganizationProfiles
                .AsNoTracking()
                .AnyAsync(organizationProfile =>
                    organizationProfile.OrganizationId == organizationId);

            if (!organizationExists)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            var events = await _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                .Where(eventModel => eventModel.OrganizationId == organizationId 
                    && eventModel.IsActive)
                .OrderByDescending(eventModel => eventModel.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<GetEventCardDto>>(events);
        }


        public async Task DeleteMyEventAsync(Guid organizationId, Guid eventId)
        {
            var eventEntity = Guard.EnsureFound(
                await _context.Events.SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive)
                );

            eventEntity.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}