using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IOrganizationMatchingService
    {
        Task AcceptVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task DeclineVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task<CreateMatchesResultDto> GenerateMyMatchesAsync(
            Guid organizationId,
            Guid eventId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<GetMatchedVolunteerCardDto>> GetMyMatchesAsync(
            Guid organizationId,
            Guid eventId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task RequestMyMatchAsync(
            Guid organizationId,
            Guid eventId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task RejectMyMatchAsync(
            Guid organizationId,
            Guid eventId,
            Guid matchId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<GetMatchedVolunteerCardDto>> GetMyMatchRequestsAsync(
            Guid organizationId,
            Guid eventId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}