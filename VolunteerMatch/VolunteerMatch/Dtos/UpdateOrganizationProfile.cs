using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Dtos
{
    public class UpdateOrganizationProfileDto
    {
        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [StringLength(200)]
        public string OrganizationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ორგანიზაციის საქმიანობის აღწერა სავალდებულოა.")]
        [StringLength(3000)]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "LinkedIn ბმულის ფორმატი არასწორია.")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "ფოტოს ბმულის ფორმატი არასწორია.")]
        public string? ProfilePhotoUrl { get; set; }
    }
}
