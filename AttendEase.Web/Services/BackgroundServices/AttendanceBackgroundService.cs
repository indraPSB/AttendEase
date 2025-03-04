using AttendEase.DB.Contexts;

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
                _logger.LogInformation("AttendanceBackgroundService is running.");
            }
        }
    }
}
