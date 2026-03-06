using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Dtos
{
    // TODO: where password and email must be changed ?
    public class UpdateOrganizationProfileDto
    {
        //[Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        //[EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        //[RegularExpression(
        //    @"^(?!.*\.\.)(?!.*\.$)[a-z0-9._%+\-]+@(?:[a-z0-9\-]+\.)+[a-z]{2,}$",
        //    ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        //public string Email { get; set; } = null!;

        //[Required(ErrorMessage = "პაროლი სავალდებულოა.")]
        //[MinLength(8, ErrorMessage = "პაროლი უნდა შედგებოდეს მინიმუმ 8 სიმბოლოსგან.")]
        //public string Password { get; set; } = null!;

        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [StringLength(200)]
        public string OrganizationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ორგანიზაციის საქმიანობის აღწერა სავალდებულოა.")]
        [StringLength(3000)]
        public string Description { get; set; } = string.Empty;
    }
}
