public interface ITagValidator
{
    Task ValidateSelectedTagIdsAsync(List<Guid> selectedTagIds);
}