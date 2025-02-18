using AttendEase.DB.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AttendEase.Shared.Services;

public interface IScheduleService
{
    Task<IEnumerable<Schedule>?> GetSchedules(CancellationToken cancellationToken = default);

    Task<IEnumerable<Schedule>?> GetSchedules(Guid userId, CancellationToken cancellationToken = default);
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

    public async Task<IEnumerable<Schedule>?> GetSchedules(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/schedules/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Schedule>? schedules = await response.Content.ReadFromJsonAsync<IEnumerable<Schedule>>(cancellationToken);

                if (schedules is null)
                {
                    _logger.LogWarning("No schedules found for user.");
                }
                else
                {
                    _logger.LogInformation("Schedules for user retrieved successfully.");
                }

                return schedules;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetSchedules(Guid) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
