using System;
using System.Collections.Generic;

namespace AttendEase.DB.Models;

public partial class Schedule
{
    public Guid Id { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? DaysOfWeek { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int AttendanceStartBefore { get; set; }

    public int AbsentAfter { get; set; }

    public bool Repeat { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
