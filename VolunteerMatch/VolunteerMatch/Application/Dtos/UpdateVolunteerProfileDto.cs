using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Application.Dtos
{
    // TODO: Email სხვა ადგილიდან უნდა დააფდეითდეს ?
    // TODO: თეგების, ფავორიტების და მეჩინგ საჯეშენების ლისტი (ნოთიფიკაციებში)?
    public class UpdateVolunteerProfileDto
    {
        [Required(ErrorMessage = "სახელი სავალდებულოა.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "სახელი 2-დან 100 სიმბოლომდე უნდა იყოს.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "გვარი სავალდებულოა.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "გვარი 2-დან 100 სიმბოლომდე უნდა იყოს.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "დაბადების თარიღი სავალდებულოა.")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "ქვეყნის მოქალაქეობა სავალდებულოა.")]
        [StringLength(100, ErrorMessage = "მოქალაქეობა ძალიან გრძელია.")]
        public string Citizenship { get; set; } = null!;

        [Required(ErrorMessage = "პროფესია სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "პროფესია ძალიან გრძელია.")]
        public string Profession { get; set; } = null!;

        [Required(ErrorMessage = "ენები სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "ენები ძალიან გრძელია.")]
        public string Languages { get; set; } = null!;

        [Required(ErrorMessage = "უნარები სავალდებულოა.")]
        [StringLength(1000, ErrorMessage = "უნარები ძალიან გრძელია.")]
        public string Skills { get; set; } = null!;

        [Required(ErrorMessage = "ინტერესები სავალდებულოა.")]
        [StringLength(1000, ErrorMessage = "ინტერესები ძალიან გრძელია.")]
        public string Interests { get; set; } = null!;

        [StringLength(500, ErrorMessage = "განათლება ძალიან გრძელია.")]
        public string? Education { get; set; }

        [Url(ErrorMessage = "ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ბმული ძალიან გრძელია.")]
        public string? ProfilePhotoUrl { get; set; }

        [Url(ErrorMessage = "ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ბმული ძალიან გრძელია.")]
        public string? LinkedInUrl { get; set; }

        [StringLength(300, ErrorMessage = "ტექნოლოგიები ძალიან გრძელია.")]
        public string? Technologies { get; set; }

        [StringLength(2000, ErrorMessage = "გამოცდილება ძალიან გრძელია.")]
        public string? Experience { get; set; }

        [StringLength(400, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string? Description { get; set; }
    }
}

