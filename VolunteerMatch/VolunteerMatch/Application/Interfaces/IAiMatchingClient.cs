using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IAiMatchingClient
    {
        Task<AiMatchResultDto> CalculateMatchAsync(
            AiVolunteerInfoDto volunteer,
            AiEventInfoDto eventInfo,
            CancellationToken cancellationToken = default);
    }
}