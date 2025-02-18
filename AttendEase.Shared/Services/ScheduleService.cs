using AttendEase.DB.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AttendEase.Shared.Services;

public interface IScheduleService
{
    Task<IEnumerable<Schedule>?> GetSchedules(CancellationToken cancellationToken = default);

    Task<Schedule?> GetSchedule(Guid id, CancellationToken cancellationToken = default);
}

public class ScheduleService(ILogger<ScheduleService> logger, HttpClient httpClient) : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<Schedule>?> GetSchedules(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/schedules");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Schedule>? schedules = await response.Content.ReadFromJsonAsync<IEnumerable<Schedule>>(cancellationToken);

                if (schedules is null)
                {
                    _logger.LogWarning("No schedules found.");
                }
                else
                {
                    _logger.LogInformation("Schedules retrieved successfully.");
                }

                return schedules;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetSchedules with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<Schedule?> GetSchedule(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/schedules/{id}");

            if (response.IsSuccessStatusCode)
            {
                Schedule? schedule = await response.Content.ReadFromJsonAsync<Schedule>(cancellationToken);

                if (schedule is null)
                {
                    _logger.LogWarning("No schedule found.");
                }
                else
                {
                    _logger.LogInformation("Schedule retrieved successfully.");
                }

                return schedule;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetSchedule with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
