using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Dtos.Matching;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class VolunteerMatchingFactory
    {
        public static AiBatchRequestDto CreateAiBatchRequest(
            VolunteerProfile volunteer,
            List<Event> candidateEvents)
        {
            return new AiBatchRequestDto
            {
                Volunteer = new AiVolunteerInfoDto
                {
                    Skills = volunteer.Skills,
                    Interests = volunteer.Interests
                },

                Events = candidateEvents
                    .Select(CreateAiEventInfo)
                    .ToList()
            };
        }



        public static List<VolunteerEventMatch> CreateRecommendedMatches(
            Guid volunteerId,
            List<Event> matchedEvents)
        {
            return matchedEvents
                .Select(eventItem => CreateRecommendedMatch(volunteerId, eventItem))
                .ToList();
        }



        private static AiEventInfoDto CreateAiEventInfo(Event eventItem)
        {
            var orderedTags = eventItem.EventTags
                .OrderBy(eventTag => eventTag.SortOrder)
                .Select(eventTag => eventTag.Tag.Name)
                .ToList();

            return new AiEventInfoDto
            {
                EventId = eventItem.EventId,
                Requirements = eventItem.Requirements,
                MainTheme = orderedTags.FirstOrDefault() ?? string.Empty,
                Tags = orderedTags
            };
        }



        private static VolunteerEventMatch CreateRecommendedMatch(
            Guid volunteerId,
            Event eventItem)
        {
            return new VolunteerEventMatch
            {
                VolunteerId = volunteerId,
                EventId = eventItem.EventId,
                RequestedByRole = UserRoles.Volunteer,
                Status = MatchStatus.Recommended
            };
        }
    }
}