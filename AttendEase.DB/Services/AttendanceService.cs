using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal class AttendanceService
{
    public static async Task<Attendance?> GetAttendance<T>(Guid userId, Guid scheduleId, DateTimeOffset timestampStart, DateTimeOffset timestampEnd, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM attendance WHERE user_id = @userId AND schedule_id = @scheduleId AND timestamp BETWEEN @timestampStart AND @timestampEnd;
            return await context.Attendances.FirstOrDefaultAsync(a => a.UserId == userId && a.ScheduleId == scheduleId && a.Timestamp >= timestampStart && a.Timestamp <= timestampEnd, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetAttendance(UserId, ScheduleId, TimestampStart, TimestampEnd) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
