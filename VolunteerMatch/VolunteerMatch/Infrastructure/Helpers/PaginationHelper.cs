using VolunteerMatch.Application.Dtos;

public static class PaginationHelper
{
    public static PagedResultDto<T> CreatePagedResult<T>(
        List<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PagedResultDto<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}