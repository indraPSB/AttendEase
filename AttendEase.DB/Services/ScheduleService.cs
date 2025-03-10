﻿using AttendEase.DB.Contexts;
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

    public static async Task<Schedule?> GetSchedule<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM schedule JOIN user ON schedule.id = user.id WHERE schedule.id = @id;
            return await context.Schedules.Include(s => s.Users).SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetSchedule(Guid) with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<bool> AddSchedule<T>(Schedule schedule, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // INSERT INTO schedule (id, name, start_date, end_date, start_time, end_time, days_of_week, latitude, longitude, location_tolerance, attendance_start_before, absent_after, attended)
            // VALUES (@id, @name, @startDateTime, @endDateTime, @startTime, @endTime, @daysOfWeek, @latitude, @longitude, @locationTolerance, @attendanceStartBefore, @absentAfter, @attended);
            await context.Schedules.AddAsync(schedule, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB AddSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> UpdateSchedule<T>(Schedule schedule, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // UPDATE schedule
            // SET name = @name, start_date = @startDateTime, end_date = @endDateTime, start_time = @startTime, end_time = @endTime, days_of_week = @daysOfWeek, latitude = @latitude, longitude = @longitude, location_tolerance = @locationTolerance, attendance_start_before = @attendanceStartBefore, absent_after = @absentAfter, attended = @attended
            // WHERE id = @id;
            context.Schedules.Update(schedule);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB UpdateSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteSchedule<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM schedule WHERE id = @id;
            Schedule? schedule = await context.Schedules.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (schedule is not null)
            {
                // DELETE FROM schedule WHERE id = @id;
                context.Schedules.Remove(schedule);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteSchedules<T>(IEnumerable<Guid> ids, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM schedule WHERE id IN @ids;
            IEnumerable<Schedule> schedules = await context.Schedules.Where(s => ids.Contains(s.Id)).ToListAsync(cancellationToken);

            if (schedules.Any())
            {
                // DELETE FROM schedule WHERE id IN @ids;
                context.Schedules.RemoveRange(schedules);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteSchedules with message, '{message}'.", ex.Message);
        }

        return false;
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

    public static async Task<bool> UpdateUserAssignment<T>(Schedule schedule, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }
        try
        {
            if (schedule is { Users: not null })
            {
                // SELECT * FROM schedule JOIN user ON schedule.id = user.id WHERE schedule.id = @scheduleId;
                Schedule? dbSchedule = await context.Schedules.Include(s => s.Users).SingleOrDefaultAsync(s => s.Id == schedule.Id, cancellationToken);

                if (dbSchedule is { Users: not null })
                {
                    dbSchedule.Users.Clear();

                    foreach (User user in schedule.Users)
                    {
                        // SELECT * FROM user WHERE id = @userId;
                        User? dbUser = await context.Users.FindAsync(user.Id);

                        if (dbUser is not null)
                        {
                            dbSchedule.Users.Add(dbUser);
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB UpdateUserAssignment with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
