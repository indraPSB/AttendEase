using AttendEase.DB.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace AttendEase.Shared.Services;

public interface IAttendanceService
{
    Task<IEnumerable<Attendance>?> GetAttendances(CancellationToken cancellationToken = default);

    Task<Attendance?> GetAttendance(GetAttendanceRequest request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAttendance(Attendance user, CancellationToken cancellationToken = default);

    Task<bool> DeleteAttendance(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteAttendances(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IEnumerable<Attendance>?> GetAttendances(Guid userId, CancellationToken cancellationToken = default);
}

public class AttendanceService(ILogger<AttendanceService> logger, HttpClient httpClient) : IAttendanceService
{
    private readonly ILogger<AttendanceService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<Attendance>?> GetAttendances(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/attendances");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Attendance>? attendances = await response.Content.ReadFromJsonAsync<IEnumerable<Attendance>>(cancellationToken);

                if (attendances is null)
                {
                    _logger.LogWarning("No attendances found.");
                }
                else
                {
                    _logger.LogInformation("Attendances retrieved successfully.");
                }

                return attendances;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetAttendances with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<Attendance?> GetAttendance(GetAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/api/attendances/user", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                Attendance? attendance = await response.Content.ReadFromJsonAsync<Attendance>(cancellationToken);

                if (attendance is null)
                {
                    _logger.LogWarning("No attendance found.");
                }
                else
                {
                    _logger.LogInformation("Attendance retrieved successfully.");
                }

                return attendance;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetAttendance with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<bool> UpdateAttendance(Attendance attendance, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/api/attendances", attendance, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Attendance updated successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared UpdateAttendance with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteAttendance(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/attendances/{id}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Attendance deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteAttendance with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<bool> DeleteAttendances(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"/api/attendances/delete", ids, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Attendances deleted successfully.");

                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared DeleteAttendances with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task<IEnumerable<Attendance>?> GetAttendances(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/attendances/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<Attendance>? attendances = await response.Content.ReadFromJsonAsync<IEnumerable<Attendance>>(cancellationToken);

                if (attendances is null)
                {
                    _logger.LogWarning("No attendances found.");
                }
                else
                {
                    _logger.LogInformation("Attendances retrieved successfully.");
                }

                return attendances;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetAttendances(Guid) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}

public class GetAttendanceRequest
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTimeOffset TimestampStart { get; set; }
    public DateTimeOffset TimestampEnd { get; set; }
}
