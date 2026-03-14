using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Infrastructure.Validators
{
    public static class OrganizationProfileValidator
    {
        public static void ValidateForUpdate(UpdateOrganizationProfileDto updateDto)
        {
            UrlValidator.ValidateLinkedInUrl(updateDto.LinkedInUrl);
        }
    }
}