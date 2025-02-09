using System;
using System.Collections.Generic;

namespace AttendEase.DB.Models;

public partial class Attendance
{
    public Guid Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public Guid UserId { get; set; }

    public Guid? ScheduleId { get; set; }

    public bool Attended { get; set; }

    public virtual Schedule? Schedule { get; set; }

    public virtual User User { get; set; } = null!;
}
