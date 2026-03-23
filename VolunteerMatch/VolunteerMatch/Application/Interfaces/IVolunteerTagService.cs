namespace VolunteerMatch.Application.Interfaces;

public interface IVolunteerTagService
{
    Task SaveVolunteerTagsAsync(Guid volunteerId, List<Guid> tagIds);
    Task SyncVolunteerTagsAsync(Guid volunteerId, List<Guid> selectedTagIds);
}

