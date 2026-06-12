using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Application.Services
{
    public class MyVolunteerService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVolunteerTagService _volunteerTagService;
        private readonly ITagValidator _tagValidator;
        private readonly IVolunteerProfileSelectionService _volunteerProfileSelectionService;

        public MyVolunteerService(
            VolunteerMatchingDbContext context, 
            IMapper mapper,
            IVolunteerTagService volunteerTagService,
            ITagValidator tagValidator,
            IVolunteerProfileSelectionService volunteerProfileSelectionService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _volunteerTagService = volunteerTagService ?? throw new ArgumentNullException(nameof(volunteerTagService));
            _tagValidator = tagValidator ?? throw new ArgumentNullException(nameof(tagValidator));
            _volunteerProfileSelectionService = volunteerProfileSelectionService ??
                throw new ArgumentNullException(nameof(volunteerProfileSelectionService));
        }

        public async Task<GetMyVolunteerProfileDto> GetMyProfileAsync(Guid volunteerId)
        {
            var profile = Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .Include(p => p.Volunteer)
                    .Include(p => p.VolunteerTags)
                    .SingleOrDefaultAsync(p => p.VolunteerId == volunteerId));

            return _mapper.Map<GetMyVolunteerProfileDto>(profile);
        }


        public async Task UpdateMyProfileAsync(Guid volunteerId, UpdateVolunteerProfileDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);
            VolunteerProfileValidator.ValidateForUpdate(updateDto);

            var profile = Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .SingleOrDefaultAsync(p => p.VolunteerId == volunteerId));

            await _tagValidator.ValidateSelectedTagIdsAsync(updateDto.SelectedTagIds);
            
            _mapper.Map(updateDto, profile);

            await _volunteerProfileSelectionService
               .SyncVolunteerSkillsAndInterestsAsync(
                   profile,
                   updateDto.SelectedSkillIds,
                   updateDto.SelectedInterestIds);

            await _volunteerTagService
                .SyncVolunteerTagsAsync(volunteerId, updateDto.SelectedTagIds);

            await _context.SaveChangesAsync();
        }
    }
}