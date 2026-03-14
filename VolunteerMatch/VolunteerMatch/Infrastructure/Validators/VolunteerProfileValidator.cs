using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Infrastructure.Validators
{
    public static class VolunteerProfileValidator
    {
        public static void ValidateForCreate(CreateVolunteerDto createDto)
        {
            ValidateBirthDate(createDto.BirthDate);
        }


        public static void ValidateForUpdate(UpdateVolunteerProfileDto updateDto)
        {
            ValidateBirthDate(updateDto.BirthDate);
            UrlValidator.ValidateLinkedInUrl(updateDto.LinkedInUrl);
        }


        private static void ValidateBirthDate(DateOnly birthDate)
        {
            if (birthDate == default)
                throw new ArgumentException("დაბადების თარიღი სავალდებულოა.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (birthDate > today)
                throw new ArgumentException("დაბადების თარიღი არ შეიძლება იყოს მომავალში.");
        }
    }
}