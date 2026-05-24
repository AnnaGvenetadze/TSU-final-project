using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Ai
{
    public class AiMatchingLogger : IAiMatchingLogger
    {
        private readonly ILogger<AiMatchingLogger> _logger;

        public AiMatchingLogger(ILogger<AiMatchingLogger> logger)
        {
            _logger = logger;
        }

        public void LogPrefilteredData(
            VolunteerProfile volunteer,
            List<Event> candidateEvents)
        {
            var volunteerTags = volunteer.VolunteerTags
                .Select(vt => vt.Tag?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name));

            _logger.LogInformation(
                "AI matching prefilter result. VolunteerId: {VolunteerId}, Skills: {Skills}, Interests: {Interests}, VolunteerTags: {VolunteerTags}, CandidateEventsCount: {CandidateEventsCount}",
                volunteer.VolunteerId,
                volunteer.Skills,
                volunteer.Interests,
                string.Join(", ", volunteerTags),
                candidateEvents.Count
            );

            foreach (var candidateEvent in candidateEvents)
            {
                var eventTags = candidateEvent.EventTags
                    .Select(et => et.Tag?.Name)
                    .Where(name => !string.IsNullOrWhiteSpace(name));

                _logger.LogInformation(
                    "AI candidate event. EventId: {EventId}, Title: {Title}, Requirements: {Requirements}, IsActive: {IsActive}, StartDate: {StartDate}, EndDate: {EndDate}, Tags: {Tags}",
                    candidateEvent.EventId,
                    candidateEvent.Title,
                    candidateEvent.Requirements,
                    candidateEvent.IsActive,
                    candidateEvent.StartDate,
                    candidateEvent.EndDate,
                    string.Join(", ", eventTags)
                );
            }
        }

        public void LogAiRequest(AiBatchRequestDto request)
        {
            _logger.LogInformation(
                "AI matching request prepared. VolunteerSkills: {Skills}, VolunteerInterests: {Interests}, EventsCount: {EventsCount}",
                request.Volunteer.Skills,
                request.Volunteer.Interests,
                request.Events.Count
            );

            foreach (var eventInfo in request.Events)
            {
                _logger.LogInformation(
                    "AI request event. EventId: {EventId}, Requirements: {Requirements}, MainTheme: {MainTheme}, Tags: {Tags}",
                    eventInfo.EventId,
                    eventInfo.Requirements,
                    eventInfo.MainTheme,
                    string.Join(", ", eventInfo.Tags)
                );
            }
        }

        public void LogAiResponse(AiBatchResponseDto response)
        {
            _logger.LogInformation(
                "AI matching response received. MatchedEventIdsCount: {MatchedEventIdsCount}, MatchedEventIds: {MatchedEventIds}",
                response.MatchedEventIds?.Count ?? 0,
                response.MatchedEventIds is null
                    ? "null"
                    : string.Join(", ", response.MatchedEventIds)
            );
        }
    }
}