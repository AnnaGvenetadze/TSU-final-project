using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Infrastructure.Helpers;
using VolunteerMatch.Infrastructure.Data;
using VolunteerMatch.Validators;


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
            var profile = Guard.EnsureFound(
                 await _context.OrganizationProfiles
                    .AsNoTracking()
                    .Include(p => p.Organization) // p.Organization (User)-დან იმეილს
                    .SingleOrDefaultAsync(p => p.OrganizationId == organizationId));

            return _mapper.Map<GetMyOrganizationProfileDto>(profile);
        }


        public async Task UpdateMyProfileAsync(Guid organizationId, UpdateOrganizationProfileDto updateDto)
        {
            ArgumentNullException.ThrowIfNull(updateDto, nameof(updateDto));
            OrganizationProfileValidator.ValidateForUpdate(updateDto);

            var profile = Guard.EnsureFound(
                await _context.OrganizationProfiles
                    .SingleOrDefaultAsync(o => o.OrganizationId == organizationId));

            _mapper.Map(updateDto, profile);

            await _context.SaveChangesAsync();
        }
    }
}
