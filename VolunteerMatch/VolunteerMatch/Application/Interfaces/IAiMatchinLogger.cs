using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IAiMatchingLogger
    {
        void LogPrefilteredData(
            VolunteerProfile volunteer,
            List<Event> candidateEvents);

        void LogAiRequest(AiBatchRequestDto request);

        void LogAiResponse(AiBatchResponseDto response);
    }
}