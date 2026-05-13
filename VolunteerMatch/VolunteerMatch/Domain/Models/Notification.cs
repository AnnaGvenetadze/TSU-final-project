namespace VolunteerMatch.Domain.Models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }

        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public byte Type { get; set; }

        public string Message { get; set; } = null!;

        public Guid? RelatedUserId { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public Event Event { get; set; } = null!;

        public User? RelatedUser { get; set; }
    }
}