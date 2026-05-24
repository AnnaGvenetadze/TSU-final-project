using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerMatchingService
    {
        Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default);
    }
}