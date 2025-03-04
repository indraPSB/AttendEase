using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendEase.Web.Services.BackgroundServices;

internal class AttendanceBackgroundService(ILogger<AttendanceBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger<AttendanceBackgroundService> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(55));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            AttendEaseDbContext? context = scope.ServiceProvider.GetService<AttendEaseDbContext>();

            if (context is not null)
            {
                _logger.LogInformation("Attendance background service is running.");

                List<Schedule> schedules = await context.Schedules.Include(s => s.Users).ToListAsync(stoppingToken);
                DateOnly date = DateOnly.FromDateTime(DateTimeOffset.Now.Date);

                foreach (Schedule schedule in schedules)
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

                    foreach (User user in schedule.Users)
                    {
                        Attendance? attendance = await context.Attendances.FirstOrDefaultAsync(a => a.UserId == user.Id && a.ScheduleId == schedule.Id && a.Timestamp >= timestampStart && a.Timestamp <= timestampEnd, cancellationToken: stoppingToken);

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

                            await context.Attendances.AddAsync(attendance, stoppingToken);
                            await context.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("Attendance created for user '{user}' in schedule '{schedule}' for timestamp '{timestamp}'.", user.Id, schedule.Id, timestamp);
                        }
                    }
                }
            }
        }
    }
}
