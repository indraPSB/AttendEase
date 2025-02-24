using AttendEase.DB.Models;

namespace AttendEase.Shared.Services;

public interface IAttendanceService
{
    Task<Attendance?> GetAttendance(GetAttendanceRequest id, CancellationToken cancellationToken = default);
}

public class GetAttendanceRequest
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTimeOffset TimestampStart { get; set; }
    public DateTimeOffset TimestampEnd { get; set; }
}
