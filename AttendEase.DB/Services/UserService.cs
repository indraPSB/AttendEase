using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal static class UserService
{
    public static async Task<IEnumerable<User>?> GetUsers<T>(ILogger<T> logger, AttendEaseDbContext context, CancellationToken ct = default)
    {
        if (ct == default)
        {
            ct = CancellationToken.None;
        }

        try
        {
            return await context.Users.ToListAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUsers with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<User?> GetUser<T>(ILogger<T> logger, AttendEaseDbContext context, Guid id, CancellationToken ct = default)
    {
        if (ct == default)
        {
            ct = CancellationToken.None;
        }

        try
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Id == id, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUser with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
