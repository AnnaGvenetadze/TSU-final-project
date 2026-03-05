using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace VolunteerMatch.Infrastructure.Helpers;

public static class DbExceptionHelper
{
    /// <summary>
    /// Detects SQL Server unique constraint violation (2601, 2627).
    /// </summary>
    public static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx)
            return sqlEx.Number is 2601 or 2627;

        return false;
    }

    /// <summary>
    /// Detects SQL Server CHECK or FK constraint violation (547).
    /// </summary>
    public static bool IsConstraintViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx)
            return sqlEx.Number == 547;

        return false;
    }
}
