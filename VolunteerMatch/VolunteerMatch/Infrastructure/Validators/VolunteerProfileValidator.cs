using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Infrastructure.Validators
{
    public static class VolunteerProfileValidator
    {
        public static void ValidateForCreate(CreateVolunteerDto createDto)
        {
            ValidateBirthDate(createDto.BirthDate);
            ValidateSelectedSkills(createDto.SelectedSkillIds);
            ValidateSelectedInterests(createDto.SelectedInterestIds);
        }


        public static void ValidateForUpdate(UpdateVolunteerProfileDto updateDto)
        {
            ValidateBirthDate(updateDto.BirthDate);
            UrlValidator.ValidateLinkedInUrl(updateDto.LinkedInUrl);
            ValidateSelectedSkills(updateDto.SelectedSkillIds);
            ValidateSelectedInterests(updateDto.SelectedInterestIds);
        }


        private static void ValidateBirthDate(DateOnly birthDate)
        {
            if (birthDate == default)
                throw new ArgumentException("დაბადების თარიღი სავალდებულოა.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (birthDate > today)
                throw new ArgumentException("დაბადების თარიღი არ შეიძლება იყოს მომავალში.");
        }

        private static void ValidateSelectedSkills(List<Guid>? selectedSkillIds)
        {
            if (selectedSkillIds is null || selectedSkillIds.Count == 0)
                throw new ArgumentException("აირჩიეთ მინიმუმ ერთი უნარი.");
            if (selectedSkillIds.Any(skillId => skillId == Guid.Empty))
                throw new ArgumentException("არჩეული უნარის Id არასწორია.");

            ValidateNoDuplicateIds(
                selectedSkillIds,
                nameof(selectedSkillIds),
                "ერთი და იგივე უნარი რამდენჯერმე არ უნდა იყოს არჩეული.");
        }

        private static void ValidateSelectedInterests(List<Guid>? selectedInterestIds)
        {
            if (selectedInterestIds is null || selectedInterestIds.Count == 0)
                throw new ArgumentException("აირჩიეთ მინიმუმ ერთი ინტერესი.");
            if (selectedInterestIds.Any(interestId => interestId == Guid.Empty))
                throw new ArgumentException("არჩეული ინტერესის Id არასწორია.");

            ValidateNoDuplicateIds(
                selectedInterestIds,
                nameof(selectedInterestIds),
                "ერთი და იგივე ინტერესი რამდენჯერმე არ უნდა იყოს არჩეული.");
        }


        private static void ValidateNoDuplicateIds(
            List<Guid>? ids,
            string fieldName,
            string errorMessage)
        {
            if (ids is null)
            {
                throw new ArgumentException($"{fieldName} სავალდებულოა.");
            }

            if (ids.Count != ids.Distinct().Count())
            {
                throw new ArgumentException(errorMessage);
            }
        }
    }
}