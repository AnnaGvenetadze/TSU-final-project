namespace VolunteerMatch.Domain.Models
{
    public class Notification
    {
        public Guid NotificationId { get; set; }

        public Guid UserId { get; set; }

        public Guid VolunteerEventMatchId { get; set; }

        public byte Type { get; set; }

        public string Message { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public VolunteerEventMatch VolunteerEventMatch { get; set; } = null!;
    }
}