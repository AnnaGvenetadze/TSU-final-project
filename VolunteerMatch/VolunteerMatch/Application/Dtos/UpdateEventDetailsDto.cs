using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Application.Dtos
{// TODO: აქ იმეილი ახალი უნდა ჩაიწეროს თუ წამოვიღო მიმდინარე ორგანიზაციიდან ?
 // TODO: სპიკერებიც დაემატება
    public class UpdateEventDetailsDto
    {
        [Required(ErrorMessage = "სათაური სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "სათაური ძალიან გრძელია.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "აღწერა სავალდებულოა.")]
        [StringLength(2000, ErrorMessage = "აღწერა ძალიან გრძელია.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "მოთხოვნები სავალდებულოა.")]
        [StringLength(1000, ErrorMessage = "მოთხოვნები ძალიან გრძელია.")]
        public string Requirements { get; set; } = null!;

        [Required(ErrorMessage = "ლოკაცია სავალდებულოა.")]
        [StringLength(100, ErrorMessage = "ლოკაცია ძალიან გრძელია.")]
        public string Location { get; set; } = null!;

        [Required(ErrorMessage = "დაწყების თარიღი სავალდებულოა.")]
        public DateTimeOffset StartDate { get; set; }

        [Required(ErrorMessage = "დასრულების თარიღი სავალდებულოა.")]
        public DateTimeOffset EndDate { get; set; }

        [Required(ErrorMessage = "დღის დაწყების დრო სავალდებულოა.")]
        public TimeOnly DailyStartTime { get; set; }

        [Required(ErrorMessage = "დღის დასრულების დრო სავალდებულოა.")]
        public TimeOnly DailyEndTime { get; set; }

        [Required(ErrorMessage = "მოხალისეების რაოდენობა სავალდებულოა.")]
        [Range(1, int.MaxValue, ErrorMessage = "მოხალისეების რაოდენობა უნდა იყოს 1-ზე მეტი ან ტოლი.")]
        public int VolunteersAmount { get; set; }

        [Required(ErrorMessage = "ბენეფიტები სავალდებულოა.")]
        [StringLength(1000, ErrorMessage = "ბენეფიტები ძალიან გრძელია.")]
        public string Benefits { get; set; } = null!;


        [Url(ErrorMessage = "ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ბმული ძალიან გრძელია.")]
        public string? MainPhotoUrl { get; set; }


        [Url(ErrorMessage = "ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ბმული ძალიან გრძელია.")]
        public string? Photo2Url { get; set; }


        [Url(ErrorMessage = "ბმულის ფორმატი არასწორია.")]
        [StringLength(500, ErrorMessage = "ბმული ძალიან გრძელია.")]
        public string? Photo3Url { get; set; }


        [StringLength(1000, ErrorMessage = "დამატებითი ინფორმაცია ძალიან გრძელია.")]
        public string? AdditionalInfo { get; set; }

        [Required(ErrorMessage = "თემატიკა სავალდებულოა.")]
        public required List<Guid> SelectedTagIds { get; set; } = [];
    }
}
