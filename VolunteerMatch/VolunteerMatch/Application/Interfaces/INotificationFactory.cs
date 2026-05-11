using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Application.Interfaces
{
    public interface INotificationFactory
    {
        Notification CreateMatchProposedByOrganization(
            Guid volunteerId,
            Event eventEntity,
            string organizationName);

        Notification CreateMatchProposedByVolunteer(
            Guid organizationId,
            Event eventEntity,
            Guid volunteerId,
            string volunteerName);

        Notification CreateMatchAcceptedByOrganization(
            Guid volunteerId,
            Event eventEntity,
            string organizationName);

        Notification CreateMatchAcceptedByVolunteer(
            Guid organizationId,
            Event eventEntity,
            Guid volunteerId,
            string volunteerName);
    }
}
