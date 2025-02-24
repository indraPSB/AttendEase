using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace AttendEase.Web.Endpoints;

internal static class AttendanceEndpoint
{
    public static WebApplication MapAttendanceEndpoints(this WebApplication app)
    {
        app.MapGet("api/attendance", GetAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        app.MapPut("api/attendance", UpdateAttendance)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business},{UserRole.Standard}" });

        return app;
    }

    private static async Task<IResult> GetAttendance([AsParameters] GetAttendanceRequest request, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        Attendance? attendance = await attendanceService.GetAttendance(request, cancellationToken);

        if (attendance is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(attendance);
    }

    private static async Task<IResult> UpdateAttendance(Attendance attendance, IAttendanceService attendanceService, CancellationToken cancellationToken)
    {
        if (await attendanceService.UpdateAttendance(attendance, cancellationToken))
        {
            return Results.Ok(attendance);
        }

        return Results.BadRequest();
    }
}
