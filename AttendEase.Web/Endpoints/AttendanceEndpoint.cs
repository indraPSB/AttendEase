using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace AttendEase.Web.Endpoints;

internal static class AttendanceEndpoint
{
    public static WebApplication MapAttendanceEndpoints(this WebApplication app)
    {
        app.MapGet("api/attendances", GetAttendances)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapGet("api/attendances/{id:guid}", GetAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        app.MapPost("api/attendances", AddAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPut("api/attendances", UpdateAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        app.MapDelete("api/attendances/{id:guid}", DeleteAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/attendances/delete", DeleteAttendances)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        return app;
    }

    public static async Task<IResult> GetAttendances(IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        IEnumerable<Attendance>? attendances = null; //TODO: await attendanceService.GetAttendances(cancellationToken);

        if (attendances is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(attendances);
    }

    public static async Task<IResult> GetAttendance([AsParameters] GetAttendanceRequest request, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        Attendance? attendance = await attendanceService.GetAttendance(request, cancellationToken);

        if (attendance is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(attendance);
    }

    public static async Task<IResult> AddAttendance(Attendance attendance, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        //TODO: if (await attendanceService.AddAttendance(attendance, cancellationToken))
        if (false)
        {
            return Results.Ok(attendance);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> UpdateAttendance(Attendance attendance, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        if (await attendanceService.UpdateAttendance(attendance, cancellationToken))
        {
            return Results.Ok(attendance);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteAttendance(Guid id, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        //TODO: if (await attendanceService.DeleteAttendance(id, cancellationToken))
        if (false)
        {
            return Results.Ok();
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteAttendances(IEnumerable<Guid> ids, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        //TODO: if (await attendanceService.DeleteAttendances(ids, cancellationToken))
        if (false)
        {
            return Results.Ok();
        }

        return Results.BadRequest();
    }
}
