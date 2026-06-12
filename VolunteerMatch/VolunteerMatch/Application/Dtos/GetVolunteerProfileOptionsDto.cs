namespace VolunteerMatch.Application.Dtos;

public class GetVolunteerProfileOptionsDto
{
    public List<SelectOptionDto> Skills { get; set; } = [];

    public List<SelectOptionDto> Interests { get; set; } = [];
}