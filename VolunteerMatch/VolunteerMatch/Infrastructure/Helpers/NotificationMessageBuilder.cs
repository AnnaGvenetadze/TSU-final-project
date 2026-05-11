namespace VolunteerMatch.Infrastructure.Helpers
{
    public class NotificationMessageBuilder
    {
        public static string MatchProposedByOrganization(string organizationName, string eventTitle)
            => $"{organizationName}-მა გამოგიგზავნა შეთავაზება \"{eventTitle}\" ღონისძიებაზე";

        public static string MatchProposedByVolunteer(string volunteerName, string eventTitle)
            => $"{volunteerName}-მა გამოგიგზავნა შეთავაზება \"{eventTitle}\" ღონისძიებაზე";

        public static string MatchAcceptedByOrganization(string organizationName, string eventTitle)
            => $"{organizationName}-მა დაადასტურა მეჩი \"{eventTitle}\" ღონისძიებაზე";

        public static string MatchAcceptedByVolunteer(string volunteerName, string eventTitle)
            => $"{volunteerName}-მა დაადასტურა მეჩი \"{eventTitle}\" ღონისძიებაზე";
    }
}
