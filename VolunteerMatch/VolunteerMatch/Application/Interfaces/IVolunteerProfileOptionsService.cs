using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerProfileOptionsService
    {
        Task<GetVolunteerProfileOptionsDto> GetVolunteerProfileOptionsAsync(
            CancellationToken cancellationToken = default);
    }
}