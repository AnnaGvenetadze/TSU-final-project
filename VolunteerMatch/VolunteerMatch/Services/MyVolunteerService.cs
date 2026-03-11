using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Services
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
            var profile = await _context.VolunteerProfiles
                .AsNoTracking()
                .Include(p => p.Volunteer)
                .SingleOrDefaultAsync(p => p.VolunteerId == volunteerId);

            if (profile is null)
                throw new KeyNotFoundException("მოხალისის პროფილი ვერ მოიძებნა.");

            return _mapper.Map<GetMyVolunteerProfileDto>(profile);
        }

        public async Task UpdateMyProfileAsync(Guid volunteerId, UpdateVolunteerProfileDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);

            if (updateDto.BirthDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("დაბადების თარიღი უნდა იყოს წარსულში.");

            var profile = await _context.VolunteerProfiles
                .SingleOrDefaultAsync(p => p.VolunteerId == volunteerId);

            if (profile is null)
                throw new KeyNotFoundException("მოხალისის პროფილი ვერ მოიძებნა.");

            _mapper.Map(updateDto, profile);

            await _context.SaveChangesAsync();
        }
    }
}