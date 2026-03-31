namespace VolunteerMatch.Application.Dtos
{
    public class GetVolunteerEventCardDto : GetEventCardDto
    {
        public bool IsFavorite { get; set; }
    }
}
