namespace VolunteerMatch.Application.Interfaces;

public interface IEventTagService
{
    Task SaveEventTagsAsync(Guid volunteerId, List<Guid> tagIds);
    Task SyncEventTagsAsync(Guid volunteerId, List<Guid> selectedTagIds);
}