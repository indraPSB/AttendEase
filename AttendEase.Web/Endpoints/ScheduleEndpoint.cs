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
        Schedule? schedule = await scheduleService.GetSchedule(id, cancellationToken);

        if (schedule is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(schedule);
    }
}
