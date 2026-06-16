using VolunteerMatch.Domain.Constants;
using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Infrastructure.Helpers
{
    public static class NotificationMessageBuilder
    {
        public static string CreateOrganizationNotificationMessage(
            VolunteerEventMatch match)
        {
            var volunteerName = $"{match.Volunteer.FirstName} {match.Volunteer.LastName}";
            var eventTitle = match.Event.Title;

            return (match.Status, match.RequestedByRole) switch
            {
                (MatchStatus.Pending, UserRoles.Volunteer) =>
$"ახალი შესაბამისობის მოთხოვნა მოხალისისგან: {volunteerName}. ღონისძიება: \"{eventTitle}\".",

                (MatchStatus.Accepted, UserRoles.Volunteer) =>
$"შესაბამისობის მოთხოვნა დადასტურებულია. მოხალისე: {volunteerName}. ღონისძიება: \"{eventTitle}\".",

                (MatchStatus.Accepted, UserRoles.Organization) =>
$"თქვენი შესაბამისობის შეთავაზება დადასტურდა. მოხალისე: {volunteerName}. ღონისძიება: \"{eventTitle}\".",

                _ =>
"შესაბამისობის სტატუსი განახლებულია."
            };
        }



        public static string CreateVolunteerNotificationMessage(
    VolunteerEventMatch match)
        {
            var organizationName = match.Event.Organization.OrganizationName;
            var eventTitle = match.Event.Title;

            return (match.Status, match.RequestedByRole) switch
            {
                (MatchStatus.Pending, UserRoles.Organization) =>
$"ახალი შესაბამისობის შეთავაზება ორგანიზაციისგან: {organizationName}. ღონისძიება: \"{eventTitle}\".",

                (MatchStatus.Accepted, UserRoles.Organization) =>
$"შესაბამისობის შეთავაზება დადასტურებულია. ორგანიზაცია: {organizationName}. ღონისძიება: \"{eventTitle}\".",

                (MatchStatus.Accepted, UserRoles.Volunteer) =>
$"თქვენი შესაბამისობის მოთხოვნა დადასტურდა. ორგანიზაცია: {organizationName}. ღონისძიება: \"{eventTitle}\".",

                _ =>
"შესაბამისობის სტატუსი განახლებულია."
            };
        }
    }
}