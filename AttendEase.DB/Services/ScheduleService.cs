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
            return await context.Schedules.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetSchedule with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<Schedule?> GetSchedule<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            return await context.Schedules.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetSchedule(Id) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
