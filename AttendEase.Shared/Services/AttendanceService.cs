using AttendEase.DB.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;

namespace AttendEase.Shared.Services;

public interface IAttendanceService
{
    Task<Attendance?> GetAttendance(GetAttendanceRequest id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAttendance(Attendance user, CancellationToken cancellationToken = default);
}

public class AttendanceService(ILogger<AttendanceService> logger, HttpClient httpClient) : IAttendanceService
{
    private readonly ILogger<AttendanceService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Attendance?> GetAttendance(GetAttendanceRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            Dictionary<string, string?> queryParams = new()
            {
                ["userId"] = request.UserId.ToString(),
                ["scheduleId"] = request.ScheduleId.ToString(),
                ["timestampStart"] = request.TimestampStart.ToString("o"),
                ["timestampEnd"] = request.TimestampEnd.ToString("o")
            };
            string url = QueryHelpers.AddQueryString("/api/attendance", queryParams);
            HttpResponseMessage response = await _httpClient.GetAsync(url);

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
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"/api/attendance", attendance, cancellationToken);

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
}

public class GetAttendanceRequest
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTimeOffset TimestampStart { get; set; }
    public DateTimeOffset TimestampEnd { get; set; }
}
