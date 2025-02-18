using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal static class ScheduleService
{
    public static async Task<IEnumerable<Schedule>?> GetSchedules<T>(ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM schedule;
            return await context.Schedules.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetSchedule with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<IEnumerable<Schedule>?> GetSchedules<T>(Guid userId, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT s.*
            // FROM schedule s
            // JOIN assignment a ON s.id = a.schedule_id
            // WHERE a.user_id = @userId;
            return await context.Schedules
                .Where(s => s.Users.Any(u => u.Id == userId))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetSchedules(Guid) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
