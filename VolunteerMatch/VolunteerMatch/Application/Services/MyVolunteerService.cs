using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Validators;

namespace VolunteerMatch.Application.Services
{
    public class MyVolunteerService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public MyVolunteerService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetMyVolunteerProfileDto> GetMyProfileAsync(Guid volunteerId)
        {
            var profile = Guard.EnsureFound(
                await _context.VolunteerProfiles
                    .AsNoTracking()
                    .Include(p => p.Volunteer)
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

            _mapper.Map(updateDto, profile);

            await _context.SaveChangesAsync();
        }
    }
}