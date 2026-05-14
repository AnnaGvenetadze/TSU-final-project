using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IAiMatchingClient
    {
        Task<AiMatchResultDto> CalculateMatchAsync(
            AiVolunteerInfoDto volunteerInfo,
            AiEventInfoDto eventInfo,
            CancellationToken cancellationToken = default);
    }
}