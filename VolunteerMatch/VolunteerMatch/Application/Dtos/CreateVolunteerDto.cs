using System.ComponentModel.DataAnnotations;
using VolunteerMatch.Infrastructure.Attributes;

namespace VolunteerMatch.Application.Dtos
{// TODO: Many-to-many tags (VolunteerTags)
    public class CreateVolunteerDto
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
        [MaxLength(100, ErrorMessage = "მოქალაქეობის ველი ძალიან გრძელია.")]
        public string Citizenship { get; set; } = null!;

        [Required(ErrorMessage = "პროფესია სავალდებულოა.")]
        [MaxLength(200, ErrorMessage = "პროფესია ძალიან გრძელია.")]
        public string Profession { get; set; } = null!;

        [Required(ErrorMessage = "ენა სავალდებულოა.")]
        [MaxLength(200, ErrorMessage = "ენების ველი ძალიან გრძელია.")]
        public string Languages { get; set; } = null!;

        [Required(ErrorMessage = "უნარი სავალდებულოა.")]
        [MaxLength(1000, ErrorMessage = "უნარების ველი ძალიან გრძელია.")]
        public string Skills { get; set; } = null!;

        [Required(ErrorMessage = "ინტერესი სავალდებულოა.")]
        [MaxLength(1000, ErrorMessage = "ინტერესების ველი ძალიან გრძელია.")]
        public string Interests { get; set; } = null!;

        //[Required(ErrorMessage = "თემატიკა სავალდებულოა.")]
        //public List<Guid> TagIds { get; set; } = new();
    }
}

