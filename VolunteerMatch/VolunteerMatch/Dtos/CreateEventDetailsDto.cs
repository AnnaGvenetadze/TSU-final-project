using System.ComponentModel.DataAnnotations;

namespace VolunteerMatch.Dtos
{// TODO: აქ იმეილი ახალი უნდა ჩაიწეროს თუ წამოვიღო მიმდინარე ორგანიზაციიდან ?
 // TODO: თემატიკის არჩევა როგორ მოხდეს ანუ თეგები როგორ დაემატოს? ცალკე 1 თეიბლად?
 // TODO: სპიკერებიც დაემატება  
    public class CreateEventDetailsDto
    {
        [Required(ErrorMessage = "სათაური სავალდებულოა.")]
        [StringLength(200, ErrorMessage = "სათაური არ უნდა აღემატებოდეს 200 სიმბოლოს.")]
        public string Title { get; set; } = null!;

        //[Required(ErrorMessage = "იმეილი სავალდებულოა.")]
        //[EmailAddress(ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        //[RegularExpression(
        //@"^(?!.*\.\.)(?!.*\.$)[a-z0-9._%+\-]+@(?:[a-z0-9\-]+\.)+[a-z]{2,}$",
        //ErrorMessage = "იმეილის ფორმატი არასწორია.")]
        //public string Email { get; set; } = null!;

        [Required(ErrorMessage = "აღწერა სავალდებულოა.")]
        [StringLength(2000, ErrorMessage = "აღწერა არ უნდა აღემატებოდეს 2000 სიმბოლოს.")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "მოთხოვნები სავალდებულოა.")]
        [StringLength(1000, ErrorMessage = "მოთხოვნები არ უნდა აღემატებოდეს 1000 სიმბოლოს.")]
        public string Requirements { get; set; } = null!;

        [Required(ErrorMessage = "ლოკაცია სავალდებულოა.")]
        [StringLength(100, ErrorMessage = "ლოკაცია არ უნდა აღემატებოდეს 100 სიმბოლოს.")]
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
        [StringLength(1000, ErrorMessage = "ბენეფიტები არ უნდა აღემატებოდეს 1000 სიმბოლოს.")]
        public string Benefits { get; set; } = null!;

        [StringLength(500, ErrorMessage = "მთავარი ფოტოს URL არ უნდა აღემატებოდეს 500 სიმბოლოს.")]
        public string? MainPhotoUrl { get; set; }

        [StringLength(500, ErrorMessage = "მეორე ფოტოს URL არ უნდა აღემატებოდეს 500 სიმბოლოს.")]
        public string? Photo2Url { get; set; }

        [StringLength(500, ErrorMessage = "მესამე ფოტოს URL არ უნდა აღემატებოდეს 500 სიმბოლოს.")]
        public string? Photo3Url { get; set; }

        [StringLength(1000, ErrorMessage = "დამატებითი ინფორმაცია არ უნდა აღემატებოდეს 1000 სიმბოლოს.")]
        public string? AdditionalInfo { get; set; }

        //public required List<string> Themes { get; set; } = [];
    }
}
