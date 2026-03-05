using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Dtos
{
    public class RegisterOrganizationDto
    {
        //Users (NOT NULL in DB)

        //[Required(ErrorMessage = "როლი სავალდებულოა.")]
        //[RegularExpression("^(მოხალისე|ორგანიზაცია)$",
        //    ErrorMessage = "ასეთი როლი არ არსებობს.")]

        [Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        [EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "პაროლი სავალდებულოა.")]
        [MinLength(8, ErrorMessage = "პაროლი უნდა შედგებოდეს მინიმუმ 8 სიმბოლოსგან.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "გაიმეორეთ პაროლი.")]
        [Compare(nameof(Password), ErrorMessage = "პაროლები არ ემთხვევა.")]
        public string ConfirmPassword { get; set; } = null!;

        // OrganizationProfiles (NOT NULL in DB)
        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [MaxLength(200, ErrorMessage = "ორგანიზაციის სახელი ძალიან გრძელია.")]
        public string OrganizationName { get; set; } = null!;

        [Required(ErrorMessage = "აღწერა სავალდებულოა.")]
        [MaxLength(500, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string Description { get; set; } = null!;
    }
}
