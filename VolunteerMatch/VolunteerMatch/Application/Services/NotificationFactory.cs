using VolunteerMatch.Application.Interfaces;
using VolunteerMatch.Domain.Models;
using VolunteerMatch.Infrastructure.Helpers;

namespace VolunteerMatch.Application.Services
{
    public class NotificationFactory : INotificationFactory
    {
        private DateTimeOffset fiveDaysLater = DateTimeOffset.UtcNow.AddDays(5);
        
        public Notification CreateMatchProposedByOrganization(
            Guid volunteerId,
            Event eventEntity,
            string organizationName)
        {
            return new Notification
            {
                UserId = volunteerId,
                EventId = eventEntity.EventId,
                Type = (byte)Domain.Constants.MatchStatus.Pending,
                Message = NotificationMessageBuilder.MatchProposedByOrganization(
                    organizationName,
                    eventEntity.Title),
                RelatedUserId = eventEntity.OrganizationId,
                ExpiresAt = fiveDaysLater < eventEntity.EndDate
                                            ? fiveDaysLater
                                            : eventEntity.EndDate
            };
        }

        public Notification CreateMatchProposedByVolunteer(
            Guid organizationId,
            Event eventEntity,
            Guid volunteerId,
            string volunteerName)
        {
            return new Notification
            {
                UserId = organizationId,
                EventId = eventEntity.EventId,
                Type = (byte)Domain.Constants.MatchStatus.Pending,
                Message = NotificationMessageBuilder.MatchProposedByVolunteer(
                    volunteerName,
                    eventEntity.Title),
                RelatedUserId = volunteerId,
                ExpiresAt = fiveDaysLater < eventEntity.EndDate
                                            ? fiveDaysLater
                                            : eventEntity.EndDate
            };
        }

        public Notification CreateMatchAcceptedByOrganization(
            Guid volunteerId,
            Event eventEntity,
            string organizationName)
        {
            return new Notification
            {
                UserId = volunteerId,
                EventId = eventEntity.EventId,
                Type = (byte)Domain.Constants.MatchStatus.Accepted,
                Message = NotificationMessageBuilder.MatchAcceptedByOrganization(
                    organizationName,
                    eventEntity.Title),
                RelatedUserId = eventEntity.OrganizationId,
                ExpiresAt = fiveDaysLater < eventEntity.EndDate
                                            ? fiveDaysLater
                                            : eventEntity.EndDate
            };
        }

        public Notification CreateMatchAcceptedByVolunteer(
            Guid organizationId,
            Event eventEntity,
            Guid volunteerId,
            string volunteerName)
        {
            return new Notification
            {
                UserId = organizationId,
                EventId = eventEntity.EventId,
                Type = (byte)Domain.Constants.MatchStatus.Accepted,
                Message = NotificationMessageBuilder.MatchAcceptedByVolunteer(
                    volunteerName,
                    eventEntity.Title),
                RelatedUserId = volunteerId,
                ExpiresAt = fiveDaysLater < eventEntity.EndDate
                                            ? fiveDaysLater
                                            : eventEntity.EndDate
            };
        }
    }
}