using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Application.Interfaces;


namespace VolunteerMatch.Application.Services
{
    public class MyOrganizationEventsService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEventTagService _eventTagService;
        private readonly ITagValidator _tagValidator;
        private readonly MatchCleanupHelper _matchCleanupHelper;

        public MyOrganizationEventsService(
            VolunteerMatchingDbContext context,
            IMapper mapper,
            IEventTagService eventTagService,
            ITagValidator tagValidator,
            MatchCleanupHelper matchCleanupHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventTagService = eventTagService ?? throw new ArgumentNullException(nameof(eventTagService));
            _tagValidator = tagValidator ?? throw new ArgumentNullException(nameof(tagValidator));
            _matchCleanupHelper = matchCleanupHelper ?? throw new ArgumentNullException(nameof(matchCleanupHelper));
        }


        public async Task<Guid> CreateEventAsync(Guid organizationId, CreateEventDetailsDto createDto)
        {
            ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));
            EventValidator.ValidateForCreate(createDto);

            Guard.EnsureFound(
                await _context.OrganizationProfiles
                    .AnyAsync(organizationProfile =>
                    organizationProfile.OrganizationId == organizationId)
            );
            await _tagValidator.ValidateSelectedTagIdsAsync(createDto.SelectedTagIds);
            
            var newEvent = _mapper.Map<Event>(createDto);
            newEvent.OrganizationId = organizationId;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                await _eventTagService.SaveEventTagsAsync(
                    newEvent.EventId, createDto.SelectedTagIds);

                await tx.CommitAsync();

                return newEvent.EventId;
            }
            catch (DbUpdateException /*ex*/)
            {
                await tx.RollbackAsync();
                throw;
                //throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }


        public async Task<GetMyOrgEventDetailsDto> GetMyEventByIdAsync(Guid organizationId, Guid eventId)
        {
            var eventEntity = Guard.EnsureFound(
                await _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                    .ThenInclude(organizationProfile =>
                        organizationProfile.Organization)
                .Include(e => e.EventTags)
                .SingleOrDefaultAsync(eventModel =>
                    eventModel.EventId == eventId &&
                    eventModel.OrganizationId == organizationId &&
                    eventModel.IsActive)
                );

            return _mapper.Map<GetMyOrgEventDetailsDto>(eventEntity);
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

            await _tagValidator.ValidateSelectedTagIdsAsync(updateDto.SelectedTagIds);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _mapper.Map(updateDto, eventEntity);
                await _context.SaveChangesAsync();
                await _eventTagService.SyncEventTagsAsync(eventId, updateDto.SelectedTagIds);

                await tx.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }



        public async Task<List<GetMyOrgEventCardDto>> GetMyEventsAsync(Guid organizationId)
        {
            Guard.EnsureFound(
                await _context.OrganizationProfiles
                .AsNoTracking()
                .AnyAsync(organizationProfile =>
                    organizationProfile.OrganizationId == organizationId)
            );

            var events = await _context.Events
                .AsNoTracking()
                .Include(eventModel => eventModel.Organization)
                 .Include(e => e.EventTags)
                    .ThenInclude(et => et.Tag)
                .Where(eventModel => eventModel.OrganizationId == organizationId
                    && eventModel.IsActive)
                .OrderByDescending(eventModel => eventModel.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<GetMyOrgEventCardDto>>(events);
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

            await _matchCleanupHelper.DeleteMatchesForEventIdAsync(eventId);
            await _context.SaveChangesAsync();
        }
    }
}