using VolunteerMatch.Domain.Models;

namespace VolunteerMatch.Application.Interfaces
{
    public interface IVolunteerProfileSelectionService
    {
        Task SaveVolunteerSkillsAndInterestsAsync(
            VolunteerProfile profile,
            List<Guid> selectedSkillIds,
            List<Guid> selectedInterestIds,
            CancellationToken cancellationToken = default);

        Task SyncVolunteerSkillsAndInterestsAsync(
            VolunteerProfile profile,
            List<Guid> selectedSkillIds,
            List<Guid> selectedInterestIds,
            CancellationToken cancellationToken = default);
    }
}