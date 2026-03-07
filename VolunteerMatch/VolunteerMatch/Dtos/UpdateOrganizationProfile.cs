using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Dtos
{
    public class UpdateOrganizationProfileDto
    {
        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "ორგანიზაციის სახელი ძალიან გრძელია.")]
        public string OrganizationName { get; set; } = null!;

        [Required(ErrorMessage = "ორგანიზაციის საქმიანობის აღწერა სავალდებულოა.")]
        [StringLength(3000, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string Description { get; set; } = null!;

        [Url(ErrorMessage = "ფოტოს ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ფოტოს ბმული ძალიან გრძელია.")]
        public string? ProfilePhotoUrl { get; set; }

        [Url(ErrorMessage = "LinkedIn ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "LinkedIn ბმული ძალიან გრძელია.")]
        public string? LinkedInUrl { get; set; }
    }
}
