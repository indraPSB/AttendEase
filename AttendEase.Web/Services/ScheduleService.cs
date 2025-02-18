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
}
