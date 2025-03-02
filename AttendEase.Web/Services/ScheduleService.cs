using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.DB.Services;
using AttendEase.Shared.Services;
using Microsoft.Extensions.Logging;
using ScheduleDBService = AttendEase.DB.Services.ScheduleService;

namespace AttendEase.Web.Services;

internal class ScheduleService(ILogger<ScheduleService> logger, AttendEaseDbContext context) : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<IEnumerable<Schedule>?> GetSchedules(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.GetSchedules(_logger, _context, cancellationToken);
    }

    public Task<Schedule?> GetSchedule(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.GetSchedule(id, _logger, _context, cancellationToken);
    }

    public async Task<bool> AddSchedule(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        schedule.Id = Guid.CreateVersion7();

        bool result = await ScheduleDBService.AddSchedule(schedule, _logger, _context, cancellationToken);

        return result;
    }

    public Task<bool> UpdateSchedule(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.UpdateSchedule(schedule, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteSchedule(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.DeleteSchedule(id, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteSchedules(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.DeleteSchedules(ids, _logger, _context, cancellationToken);
    }

    public Task<IEnumerable<Schedule>?> GetSchedules(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.GetSchedules(userId, _logger, _context, cancellationToken);
    }

    public Task<bool> UpdateUserAssignment(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return ScheduleDBService.UpdateUserAssignment(schedule, _logger, _context, cancellationToken);
    }
}
