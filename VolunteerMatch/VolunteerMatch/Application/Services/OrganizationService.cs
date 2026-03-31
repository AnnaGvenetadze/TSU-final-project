using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Infrastructure.Helpers;
using AutoMapper;
using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Services
{
    public class OrganizationService
    {
        private readonly VolunteerMatchingDbContext _context;
        private readonly IMapper _mapper;

        public OrganizationService(VolunteerMatchingDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<GetOrganizationProfileDto> GetProfileByIdAsync(Guid organizationId)
        {
            var profile = Guard.EnsureFound(
                 await _context.OrganizationProfiles
                    .AsNoTracking()
                    .Include(p => p.Organization) // p.Organization (User)-დან იმეილს
                    .SingleOrDefaultAsync(p => p.OrganizationId == organizationId));

            return _mapper.Map<GetOrganizationProfileDto>(profile);
        }
    }
}
