namespace VolunteerMatch.Application.Interfaces
{
    public interface IEventCapacityService
    {
        Task<int> GetAcceptedVolunteersCountAsync(
            Guid eventId,
            CancellationToken cancellationToken = default);

        Task<bool> IsFilledAsync(
            Guid eventId,
            int volunteersAmount,
            CancellationToken cancellationToken = default);

        Task EnsureNotFilledAsync(
            Guid eventId,
            int volunteersAmount,
            CancellationToken cancellationToken = default);
    }
}