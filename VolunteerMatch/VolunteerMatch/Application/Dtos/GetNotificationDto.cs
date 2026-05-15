namespace VolunteerMatch.Application.Dtos
{
    public class GetNotificationDto
    {
        public Guid NotificationId { get; set; }

        public Guid VolunteerEventMatchId { get; set; }

        public required string Message { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public required GetEventCardDto Event { get; set; }
    }
}

