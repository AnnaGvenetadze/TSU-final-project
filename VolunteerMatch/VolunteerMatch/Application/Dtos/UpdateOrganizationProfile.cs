using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Application.Dtos
{
    // TODO: იმეილი განახლდეს სხვა ადგილიდან?
    public class UpdateOrganizationProfileDto
    {
        [Required(ErrorMessage = "ორგანიზაციის სახელი სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "ორგანიზაციის სახელი ძალიან გრძელია.")]
        public string OrganizationName { get; set; } = null!;

        [Required(ErrorMessage = "ორგანიზაციის საქმიანობის აღწერა სავალდებულოა.")]
        [StringLength(3000, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string Description { get; set; } = null!;

        public string? ProfilePhotoUrl { get; set; }

        public string? LinkedInUrl { get; set; }
    }
}
