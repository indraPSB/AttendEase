using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace AttendEase.Web.Services.BackgroundServices;

internal class AttendanceBackgroundService(ILogger<AttendanceBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger<AttendanceBackgroundService> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Attendance background service is running.");

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IScheduleService? scheduleService = scope.ServiceProvider.GetService<IScheduleService>();

            if (scheduleService is not null)
            {
                IEnumerable<Schedule>? schedules = await scheduleService.GetSchedules(stoppingToken);

                if (schedules is not null)
                {
                    DateOnly date = DateOnly.FromDateTime(DateTimeOffset.Now.Date);

                    foreach (Schedule schedule in schedules)
                    {
                        Schedule? scheduleWithUsers = await scheduleService.GetSchedule(schedule.Id, stoppingToken);

                        if (scheduleWithUsers is not null)
                        {
                            DaysOfWeek currentDay = date.DayOfWeek switch
                            {
                                DayOfWeek.Monday => DaysOfWeek.Monday,
                                DayOfWeek.Tuesday => DaysOfWeek.Tuesday,
                                DayOfWeek.Wednesday => DaysOfWeek.Wednesday,
                                DayOfWeek.Thursday => DaysOfWeek.Thursday,
                                DayOfWeek.Friday => DaysOfWeek.Friday,
                                DayOfWeek.Saturday => DaysOfWeek.Saturday,
                                DayOfWeek.Sunday => DaysOfWeek.Sunday,
                                _ => DaysOfWeek.None
                            };

                            if (!string.IsNullOrEmpty(schedule.DaysOfWeek))
                            {
                                if (!Enum.TryParse(schedule.DaysOfWeek, out DaysOfWeek scheduledDays))
                                {
                                    continue;
                                }

                                if ((scheduledDays & currentDay) == 0)
                                {
                                    continue;
                                }
                            }

                            TimeOnly time = schedule.StartTime ?? new TimeOnly();
                            DateTimeOffset timestamp = new(date, time, TimeSpan.Zero);
                            DateTimeOffset timestampStart = timestamp.AddMinutes(-schedule.AttendanceStartBefore);
                            DateTimeOffset timestampEnd = timestamp.AddMinutes(schedule.AbsentAfter);

                            foreach (User user in scheduleWithUsers.Users)
                            {
                                IAttendanceService? iAttendanceService = scope.ServiceProvider.GetService<IAttendanceService>();

                                if (iAttendanceService is AttendanceService attendanceService)
                                {
                                    GetAttendanceRequest request = new()
                                    {
                                        UserId = user.Id,
                                        ScheduleId = schedule.Id,
                                        TimestampStart = timestampStart,
                                        TimestampEnd = timestampEnd
                                    };
                                    Attendance? attendance = await attendanceService.GetAttendance(request, stoppingToken);

                                    if (attendance is null)
                                    {
                                        attendance = new Attendance
                                        {
                                            Id = Guid.CreateVersion7(),
                                            Timestamp = timestamp,
                                            UserId = user.Id,
                                            ScheduleId = schedule.Id,
                                            Attended = false
                                        };

                                        bool success = await attendanceService.AddAttendance(attendance, stoppingToken);

                                        if (success)
                                        {
                                            _logger.LogInformation("Attendance created for user '{user}' in schedule '{schedule}' for timestamp '{timestamp}'.", user.Id, schedule.Id, timestamp);
                                        }
                                    }
                                }
                            }
                        } 
                    }
                }
            }
        }
    }
}
