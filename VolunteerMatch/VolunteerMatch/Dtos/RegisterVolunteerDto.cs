using System.ComponentModel.DataAnnotations;
using VolunteerMatch.Models;

namespace VolunteerMatch.Dtos
{
    // TODO: რეგისტრაციის დთო-ებსა და თეიბლებში როლები საჭიროა ?
    public class RegisterVolunteerDto
    {
        //Users (NOT NULL in DB)

        //[Required(ErrorMessage = "როლი სავალდებულოა.")]
        //[RegularExpression("^(მოხალისე|ორგანიზაცია)$",
        //    ErrorMessage = "ასეთი როლი არ არსებობს.")]
        //public string Role { get; set; } = null!;

        [Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        [EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        [RegularExpression(
            @"^(?!.*\.\.)(?!.*\.$)[a-z0-9._%+\-]+@(?:[a-z0-9\-]+\.)+[a-z]{2,}$",
            ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "პაროლი სავალდებულოა.")]
        [MinLength(8, ErrorMessage = "პაროლი უნდა შედგებოდეს მინიმუმ 8 სიმბოლოსგან.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "გაიმეორეთ პაროლი.")]
        [Compare(nameof(Password), ErrorMessage = "პაროლები არ ემთხვევა.")]
        public string ConfirmPassword { get; set; } = null!;

        // VolunteerProfiles (NOT NULL in DB)
        [Required(ErrorMessage = "სახელი სავალდებულოა.")]
        [MaxLength(100, ErrorMessage = "სახელი ძალიან გრძელია.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "გვარი სავალდებულოა.")]
        [MaxLength(100, ErrorMessage = "გვარი ძალიან გრძელია.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "დაბადების თარიღი სავალდებულოა.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "მოქალაქეობა სავალდებულოა.")]
        public string Citizenship { get; set; } = null!;

        [Required(ErrorMessage = "პროფესია სავალდებულოა.")]
        [MaxLength(200, ErrorMessage = "პროფესია ძალიან გრძელია.")]
        public string Profession { get; set; } = null!;

        [Required(ErrorMessage = "ენა სავალდებულოა.")]
        [MaxLength(500, ErrorMessage = "ენების ველი ძალიან გრძელია.")]
        public string Languages { get; set; } = null!;

        [Required(ErrorMessage = "უნარი სავალდებულოა.")]
        public string Skills { get; set; } = null!;

        [Required(ErrorMessage = "ინტერესი სავალდებულოა.")]
        public string Interests { get; set; } = null!;

        // Many-to-many tags (VolunteerTags)
        [Required(ErrorMessage = "თემატიკა სავალდებულოა.")]
        public List<Guid> TagIds { get; set; } = new();
    }
}

