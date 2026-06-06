using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Matching;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerMatchingService
    {
        Task AcceptOrganizationMatchRequestAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task DeclineOrganizationMatchRequestAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<GetMatchedEventCardDto>> GetMyMatchesAsync(
           Guid currentUserId,
           int page,
           int pageSize,
           CancellationToken cancellationToken = default);

        Task RequestMyMatchAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task RejectMyMatchAsync(
            Guid volunteerId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<GetMatchedEventCardDto>> GetMyMatchRequestsAsync(
            Guid volunteerId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}