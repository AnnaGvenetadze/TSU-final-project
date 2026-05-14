using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Infrastructure.Ai
{
    public class OpenAiMatchingClient : IAiMatchingClient
    {
        public Task<AiMatchResultDto> CalculateMatchAsync(
            AiVolunteerInfoDto volunteerInfo, 
            AiEventInfoDto eventInfo, 
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
