using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace AttendEase.Web.Endpoints;

internal static class ScheduleEndpoint
{
    public static WebApplication MapScheduleEndpoints(this WebApplication app)
    {
        app.MapGet("api/schedules", GetSchedules)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapGet("api/schedules/{id:guid}", GetSchedule)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/schedules", AddSchedule)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPut("api/schedules", UpdateSchedule)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapDelete("api/schedules/{id:guid}", DeleteSchedule)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/schedules/delete", DeleteSchedules)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapGet("api/schedules/user/{userId:guid}", GetSchedulesForUser)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        app.MapPut("api/schedules/user/", UpdateUserAssignment)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        return app;
    }

    public static async Task<IResult> GetSchedules(IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        IEnumerable<Schedule>? schedules = await scheduleService.GetSchedules(cancellationToken);

        if (schedules is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(schedules);
    }

    public static async Task<IResult> GetSchedule(Guid id, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        Schedule? schedule = null; //TODO: await scheduleService.GetSchedule(id, cancellationToken);

        if (schedule is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(schedule);
    }

    public static async Task<IResult> AddSchedule(Schedule schedule, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        //TODO: if (await scheduleService.AddSchedule(schedule, cancellationToken))
        if (false)
        {
            return Results.Created($"/api/schedules/{schedule.Id}", schedule);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> UpdateSchedule(Schedule schedule, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        //TODO: if (await scheduleService.UpdateSchedule(schedule, cancellationToken))
        if (false)
        {
            return Results.Ok(schedule);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteSchedule(Guid id, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        //TODO: if (await scheduleService.DeleteSchedule(id, cancellationToken))
        if (false)
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteSchedules(List<Guid> ids, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        //TODO: if (await scheduleService.DeleteSchedules(ids, cancellationToken))
        if (false)
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> GetSchedulesForUser(Guid userId, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        IEnumerable<Schedule>? schedules = await scheduleService.GetSchedules(userId, cancellationToken);

        if (schedules is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(schedules);
    }

    public static async Task<IResult> UpdateUserAssignment(Schedule schedule, IScheduleService scheduleService, CancellationToken cancellationToken)
    {
        if (schedule is { Users: not null })
        {
            //TODO: if (await scheduleService.UpdateUserAssignment(schedule, cancellationToken))
            if (false)
            {
                return Results.NoContent();
            }
        }

        return Results.BadRequest();
    }
}
