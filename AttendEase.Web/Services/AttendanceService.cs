using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using AttendanceDBService = AttendEase.DB.Services.AttendanceService;

namespace AttendEase.Web.Services;

internal class AttendanceService(ILogger<AttendanceService> logger, AttendEaseDbContext context) : IAttendanceService
{
    private readonly ILogger<AttendanceService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<IEnumerable<Attendance>?> GetAttendances(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;

        }

        return AttendanceDBService.GetAttendances(_logger, _context, cancellationToken);
    }

    public Task<Attendance?> GetAttendance(GetAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return AttendanceDBService.GetAttendance(request.UserId, request.ScheduleId, request.TimestampStart, request.TimestampEnd, _logger, _context, cancellationToken);
    }

    public Task<bool> AddAttendance(Attendance attendance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        attendance.Id = Guid.CreateVersion7();

        return AttendanceDBService.AddAttendance(attendance, _logger, _context, cancellationToken);
    }

    public Task<bool> UpdateAttendance(Attendance attendance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return AttendanceDBService.UpdateAttendance(attendance, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteAttendance(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return AttendanceDBService.DeleteAttendance(id, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteAttendances(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return AttendanceDBService.DeleteAttendances(ids, _logger, _context, cancellationToken);
    }

    public Task<IEnumerable<Attendance>?> GetAttendances(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;

        }

        return AttendanceDBService.GetAttendances(userId, _logger, _context, cancellationToken);
    }
}
