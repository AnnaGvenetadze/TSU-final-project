using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IAiMatchingClient
    {
        Task<AiBatchResponseDto> GetMatchedEventIdsForVolunteerAsync(
            AiBatchRequestDto request,
            CancellationToken cancellationToken = default);
    }
}