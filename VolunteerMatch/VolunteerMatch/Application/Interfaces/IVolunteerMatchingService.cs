namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerMatchingService
    {
        Task GenerateMyMatchesAsync(
            Guid currentUserId,
            CancellationToken cancellationToken = default);
    }
}