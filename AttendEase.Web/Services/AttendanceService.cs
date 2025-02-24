using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using AttendanceDBService = AttendEase.DB.Services.AttendanceService;

namespace AttendEase.Web.Services;

internal class AttendanceService(ILogger<AttendanceService> logger, AttendEaseDbContext context) : IAttendanceService
{
    private readonly ILogger<AttendanceService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<Attendance?> GetAttendance(GetAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return AttendanceDBService.GetAttendance(request.UserId, request.ScheduleId, request.TimestampStart, request.TimestampEnd, _logger, _context, cancellationToken);
    }
}
