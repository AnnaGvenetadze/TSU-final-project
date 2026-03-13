using VolunteerMatch.Dtos;

namespace VolunteerMatch.Validators
{
    public static class OrganizationProfileValidator
    {
        public static void ValidateForUpdate(UpdateOrganizationProfileDto updateDto)
        {
            UrlValidator.ValidateLinkedInUrl(updateDto.LinkedInUrl);
        }
    }
}