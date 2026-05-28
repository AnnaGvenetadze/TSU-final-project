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
$"{volunteerName}-მა გამოგიგზავნათ დამეჩვის მოთხოვნა \"{eventTitle}\" ღონისძიებაზე.",

                (MatchStatus.Accepted, UserRoles.Volunteer) =>
$"თქვენ დაადასტურეთ {volunteerName}-ის დამეჩვის მოთხოვნა \"{eventTitle}\" ღონისძიებაზე.",

                (MatchStatus.Accepted, UserRoles.Organization) =>
$"{volunteerName}-მა დაადასტურა თქვენი დამეჩვის შეთავაზება \"{eventTitle}\" ღონისძიებაზე.",

                _ => 
"მეჩის სტატუსი განახლებულია."
            };
        }
    }
}