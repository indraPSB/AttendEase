using AttendEase.Shared.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AttendEase.Services;

internal class AuthService(ILogger<AuthService> logger, HttpClient httpClient) : IAuthService
{
    const string Key = "jwt_token";

    private readonly ILogger<AuthService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string?> GetToken(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return await SecureStorage.GetAsync(Key);
    }

    public async Task<bool> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            if (request is { Email: not null, Password: not null })
            {
                JsonContent requestContent = JsonContent.Create(request);
                HttpResponseMessage response = await _httpClient.PostAsync("api/auth/login", requestContent, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);

                    if (result is { Token: not null })
                    {
                        await SecureStorage.SetAsync(Key, result.Token);

                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No token found in MAUI AuthService.");
                    }
                }
            }
            else
            {
                _logger.LogWarning("Login request is null in MAUI AuthService.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MAUI Login with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public Task Logout(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        SecureStorage.Remove(Key);
        _httpClient.DefaultRequestHeaders.Authorization = null;

        return Task.CompletedTask;
    }
}
