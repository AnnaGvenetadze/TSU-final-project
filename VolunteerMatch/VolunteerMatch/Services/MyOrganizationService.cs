using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;
using VolunteerMatch.Mappings;

namespace VolunteerMatch.Services
{
    public class MyOrganizationService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public MyOrganizationService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<GetMyOrganizationProfileDto> GetMyProfileAsync(Guid organizationId)
        {
            var profile = await _context.OrganizationProfiles
                .AsNoTracking()
                .Include(p => p.Organization) // p.Organization (User)-დან იმეილს
                .SingleOrDefaultAsync(p => p.OrganizationId == organizationId);

            if (profile is null)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            return _mapper.Map<GetMyOrganizationProfileDto>(profile);
        }


        public async Task UpdateMyProfileAsync(Guid organizationId, UpdateOrganizationProfileDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto);

            var profile = await _context.OrganizationProfiles
                .SingleOrDefaultAsync(o => o.OrganizationId == organizationId);

            if (profile is null)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            _mapper.Map(updateDto, profile);

            await _context.SaveChangesAsync();
        }
    }
}
