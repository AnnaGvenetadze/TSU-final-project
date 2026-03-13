using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;
using VolunteerMatch.Infrastructure.Validators;

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
            EventValidator.ValidateForCreate(createDto);

            var organizationExists = await _context.OrganizationProfiles
                .AnyAsync(organizationProfile => 
                    organizationProfile.OrganizationId == organizationId);

            if (!organizationExists)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            var newEvent = _mapper.Map<Event>(createDto);
            newEvent.OrganizationId = organizationId;

            EventValidator.TrimEntityTextFields(newEvent);

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return newEvent.EventId;
        }


        public async Task<GetEventDetailsDto> GetMyEventByIdAsync(Guid organizationId, Guid eventId)
        {
            var eventEntity = await _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                .ThenInclude(organizationProfile =>
                        organizationProfile.Organization)
                .SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive);

            if (eventEntity is null)
                throw new KeyNotFoundException("აქტიური ივენთი ვერ მოიძებნა.");

            return _mapper.Map<GetEventDetailsDto>(eventEntity);
        }

        public async Task UpdateMyEventAsync(Guid organizationId, Guid eventId, UpdateEventDetailsDto updateDto)
        {
            EventValidator.ValidateForUpdate(updateDto);

            var eventEntity = await _context.Events
                .SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive);

            if (eventEntity is null)
                throw new KeyNotFoundException("აქტიური ივენთი ვერ მოიძებნა.");

            _mapper.Map(updateDto, eventEntity);
            EventValidator.TrimEntityTextFields(eventEntity);

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
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<GetEventCardDto>>(events);
        }


        public async Task DeleteMyEventAsync(Guid organizationId, Guid eventId)
        {
            var eventEntity = await _context.Events
                .SingleOrDefaultAsync(e =>
                    e.EventId == eventId &&
                    e.OrganizationId == organizationId &&
                    e.IsActive);

            if (eventEntity is null)
                throw new KeyNotFoundException("აქტიური ივენთი ვერ მოიძებნა.");

            eventEntity.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}