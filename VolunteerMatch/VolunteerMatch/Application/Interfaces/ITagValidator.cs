namespace VolunteerMatch.Application.Interfaces;

public interface ITagValidator
{
    Task ValidateSelectedTagIdsAsync(List<Guid> selectedTagIds);
}