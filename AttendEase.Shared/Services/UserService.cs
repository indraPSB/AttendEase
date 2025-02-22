using AttendEase.DB.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AttendEase.Shared.Services;

public interface IUserService
{
    Task<IEnumerable<User>?> GetUsers(CancellationToken cancellationToken = default);

    Task<User?> GetUser(Guid id, CancellationToken cancellationToken = default);

    Task<bool> AddUser(User user, CancellationToken cancellationToken = default);

    Task<bool> UpdateUser(User user, CancellationToken cancellationToken = default);

    Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken = default);

    Task<bool> DeleteUsers(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

public class UserService(ILogger<UserService> logger, HttpClient httpClient) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<IEnumerable<User>?> GetUsers(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/users");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<User>? users = await response.Content.ReadFromJsonAsync<IEnumerable<User>>(cancellationToken);

                if (users is null)
                {
                    _logger.LogWarning("No users found.");
                }
                else
                {
                    _logger.LogInformation("Users retrieved successfully.");
                }

                return users;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetUsers with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                User? user = await response.Content.ReadFromJsonAsync<User>(cancellationToken);

                if (user is null)
                {
                    _logger.LogWarning("No user found.");
                }
                else
                {
                    _logger.LogInformation("User retrieved successfully.");
                }

                return user;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Shared GetUser with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
