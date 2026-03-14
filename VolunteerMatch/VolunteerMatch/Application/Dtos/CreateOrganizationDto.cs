using System.ComponentModel.DataAnnotations;
using VolunteerMatch.Infrastructure.Attributes;

namespace VolunteerMatch.Application.Dtos
{
    public class CreateOrganizationDto
    {
        //Users (NOT NULL in DB)
        [Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        [ValidEmail]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "იმეილის სიგრძე უნდა იყოს 3-დან 100 სიმბოლომდე.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "პაროლი სავალდებულოა.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "პაროლი უნდა შეიცავდეს 8-დან 100 სიმბოლომდე.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "გაიმეორეთ პაროლი.")]
        [Compare(nameof(Password), ErrorMessage = "პაროლები არ ემთხვევა.")]
        public string ConfirmPassword { get; set; } = null!;

        // OrganizationProfiles (NOT NULL in DB)
        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [MaxLength(200, ErrorMessage = "ორგანიზაციის სახელი ძალიან გრძელია.")]
        public string OrganizationName { get; set; } = null!;

        [Required(ErrorMessage = "აღწერა სავალდებულოა.")]
        [MaxLength(3000, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string Description { get; set; } = null!;
    }
}
