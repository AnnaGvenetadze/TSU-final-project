using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Matching;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerMatchingService
    {
        Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<GetMatchedEventCardDto>> GetMyMatchesAsync(
           Guid currentUserId,
           int page,
           int pageSize,
           CancellationToken cancellationToken = default);
    }
}