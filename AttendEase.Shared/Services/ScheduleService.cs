using AttendEase.DB.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AttendEase.Shared.Services;

public interface IScheduleService
{
    Task<IEnumerable<Schedule>?> GetSchedules(CancellationToken cancellationToken = default);

    Task<Schedule?> GetSchedule(Guid id, CancellationToken cancellationToken = default);

    Task<bool> AddSchedule(Schedule schedule, CancellationToken cancellationToken = default);

    Task<bool> UpdateSchedule(Schedule schedule, CancellationToken cancellationToken = default);

    Task<bool> DeleteSchedule(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteSchedules(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IEnumerable<Schedule>?> GetSchedules(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> UpdateUserAssignment(Schedule schedule, CancellationToken cancellationToken = default);
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
            _logger.LogError(ex, "Error in Shared GetSchedule(Guid) with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<bool> AddSchedule(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/schedules", schedule, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Schedule added successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared AddSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> UpdateSchedule(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/schedules", schedule, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Schedule updated successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared UpdateSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteSchedule(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/schedules/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Schedule deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteSchedule with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteSchedules(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/schedules/delete", ids, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Schedules deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteSchedules with message, '{message}'.", ex.Message);
        }

        return false;
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

    public async Task<bool> UpdateUserAssignment(Schedule schedule, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync("/api/schedules/user", schedule, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("User assignment updated successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared UpdateUserAssignment with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
