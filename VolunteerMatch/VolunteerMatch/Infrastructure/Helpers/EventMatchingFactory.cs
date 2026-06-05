using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class EventMatchingFactory
    {
        public static AiBatchRequestForEventDto CreateAiBatchRequest(
            Event eventItem,
            List<VolunteerProfile> candidateVolunteers)
        {
            return new AiBatchRequestForEventDto
            {
                Event = CreateAiEventInfo(eventItem),

                Volunteers = candidateVolunteers
                    .Select(CreateAiVolunteerInfo)
                    .ToList()
            };
        }



        public static List<VolunteerEventMatch> CreateRecommendedMatches(
            Guid eventId,
            List<VolunteerProfile> matchedVolunteers)
        {
            return matchedVolunteers
                .Select(volunteer => CreateRecommendedMatch(eventId, volunteer))
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



        private static AiVolunteerInfoForEventDto CreateAiVolunteerInfo(
            VolunteerProfile volunteer)
        {
            return new AiVolunteerInfoForEventDto
            {
                VolunteerId = volunteer.VolunteerId,
                Skills = volunteer.Skills,
                Interests = volunteer.Interests
            };
        }



        private static VolunteerEventMatch CreateRecommendedMatch(
            Guid eventId,
            VolunteerProfile volunteer)
        {
            return new VolunteerEventMatch
            {
                VolunteerId = volunteer.VolunteerId,
                EventId = eventId,
                RequestedByRole = UserRoles.Organization,
                Status = MatchStatus.Recommended
            };
        }
    }
}