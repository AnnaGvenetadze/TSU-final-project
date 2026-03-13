using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VolunteerMatch.Infrastructure.Attributes
{
    public class ValidEmailAttribute : ValidationAttribute
    {
        private static readonly Regex EmailRegex = new(
            @"^(?!.*\.\.)(?!.*\.$)[a-z0-9._%+\-]+@(?:[a-z0-9\-]+\.)+[a-z]{2,}$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is string email && EmailRegex.IsMatch(email))
                return ValidationResult.Success;

            return new ValidationResult("იმეილის ფორმატი არასწორია.");
        }
    }
}