using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Application.Dtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        [EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        [RegularExpression(
            @"^(?!.*\.\.)(?!.*\.$)[a-z0-9._%+\-]+@(?:[a-z0-9\-]+\.)+[a-z]{2,}$",
            ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "პაროლი სავალდებულოა.")]
        [MinLength(8, ErrorMessage = "პაროლი უნდა შედგებოდეს მინიმუმ 8 სიმბოლოსგან.")]
        public string Password { get; set; } = null!;
    }
}
