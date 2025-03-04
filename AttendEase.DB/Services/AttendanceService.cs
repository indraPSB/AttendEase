﻿using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal class AttendanceService
{
    public static async Task<IEnumerable<Attendance>?> GetAttendances<T>(ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM attendance;
            return await context.Attendances.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetAttendances with message, '{message}'.", ex.Message);
        }

        return null;
    }

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

    public static async Task<bool> UpdateAttendance<T>(Attendance attendance, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // UPDATE attendance SET status = @status WHERE user_id = @userId AND schedule_id = @scheduleId AND timestamp = @timestamp AND attended  = @attended;
            context.Attendances.Update(attendance);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB UpdateAttendance(Attendance) with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteAttendance<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM attendance WHERE id = @id;
            Attendance? attendance = await context.Attendances.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

            if (attendance is not null)
            {
                // DELETE FROM attendance WHERE id = @id;
                context.Attendances.Remove(attendance);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteAttendance with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteAttendances<T>(IEnumerable<Guid> ids, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM attendance WHERE id IN @ids;
            IEnumerable<Attendance> attendances = await context.Attendances.Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);

            if (attendances.Any())
            {
                // DELETE FROM attendance WHERE id IN @ids;
                context.Attendances.RemoveRange(attendances);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteAttendances with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
