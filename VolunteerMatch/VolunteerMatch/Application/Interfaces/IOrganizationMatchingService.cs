namespace VolunteerMatch.Application.Interfaces
{
    public interface IOrganizationMatchingService
    {
        Task AcceptVolunteerMatchRequestAsync(
            Guid organizationId,
            Guid matchId,
            CancellationToken cancellationToken = default);
    }
}