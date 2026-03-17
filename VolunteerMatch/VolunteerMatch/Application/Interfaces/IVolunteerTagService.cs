public interface IVolunteerTagService
{
    Task SaveVolunteerTags(Guid volunteerId, List<Guid> tagIds);
    Task SyncVolunteerTagsAsync(Guid volunteerId, List<Guid> selectedTagIds);
}

