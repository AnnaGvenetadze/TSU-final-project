using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Services
{
    public class VolunteerService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public VolunteerService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetVolunteerProfileDto> GetVolunteerProfileAsync(Guid volunteerId)
        {
            var profile = await _context.VolunteerProfiles
                .AsNoTracking()
                .Include(p => p.Volunteer)
                .SingleOrDefaultAsync(v => v.VolunteerId == volunteerId);

            if (profile is null)
                throw new KeyNotFoundException("მოხალისის პროფილი ვერ მოიძებნა.");

            return _mapper.Map<GetVolunteerProfileDto>(profile);
        }
    }
}