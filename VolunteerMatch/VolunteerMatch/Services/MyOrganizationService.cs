using Microsoft.EntityFrameworkCore;
using VolunteerMatch.Dtos;
using VolunteerMatch.Models;

namespace VolunteerMatch.Services
{
    public class MyOrganizationService
    {
        private readonly VolunteerMatchingDbContext _context;

        public MyOrganizationService(VolunteerMatchingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetMyOrganizationProfileDto> GetMyProfileAsync(Guid organizationId)
        {
            var profile = await _context.OrganizationProfiles
                .AsNoTracking()
                .Include(p => p.Organization) // User entity (email აქედან მოდის)
                .SingleOrDefaultAsync(p => p.OrganizationId == organizationId);

            return MapMyProfileToDto(profile);
        }

        private GetMyOrganizationProfileDto MapMyProfileToDto(OrganizationProfile? profile)
        {
            if (profile is null)
                throw new KeyNotFoundException("ორგანიზაციის პროფილი ვერ მოიძებნა.");

            return new GetMyOrganizationProfileDto
            {
                OrganizationId = profile.OrganizationId, // აქ ამის დაბრუნება რად მინდა?
                OrganizationName = profile.OrganizationName,
                Description = profile.Description,
                Email = profile.Organization.Email,
                LinkedInUrl = profile.LinkedInUrl,
                ProfilePhotoUrl = profile.ProfilePhotoUrl
                // TODO: ივენთების ფუნქციონალის შექმნის მერე ივენთების ლისტიც უნდა დაბრუნდეს
            };
        }
    }
}
