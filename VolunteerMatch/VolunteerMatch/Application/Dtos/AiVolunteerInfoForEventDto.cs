// TODO: VolunteerInfoDto-ად გადაარქმევ და მხოლოდ ამას გამოვიყენებთ ორივე მხარეს
namespace VolunteerMatch.Application.Dtos
{
    public class AiVolunteerInfoForEventDto
    {
        public required Guid VolunteerId { get; set; }

        public required string Skills { get; set; }

        public required string Interests { get; set; }

        public required string Profession { get; set; }

        public string? Experience { get; set; }
    }
}