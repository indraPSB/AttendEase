using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

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

        string? token = null;

        try
        {
            token = await SecureStorage.GetAsync(Key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MAUI AuthService.GetToken with message, '{message}'.", ex.Message);
        }

        return token;
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
            _logger.LogError(ex, "Error in MAUI AuthService.Login with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public Task Logout(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            SecureStorage.Remove(Key);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MAUI AuthService.Logout with message, '{message}'.", ex.Message);
        }

        return Task.CompletedTask;
    }

    public async Task<User?> GetUser(CancellationToken cancellationToken = default)
    {
        string? token = await GetToken(cancellationToken);

        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        // Check if token has expired
        if (jwtToken.ValidTo < DateTime.UtcNow)
        {
            return null;
        }

        string roleClaimName = tokenHandler.OutboundClaimTypeMap[ClaimTypes.Role];

        if (Guid.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value, out Guid id)
            && jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value is string name
            && jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value is string email
            && jwtToken.Claims.FirstOrDefault(c => c.Type == roleClaimName)?.Value is string role)
        {
            return new User()
            {
                Id = id,
                Name = name,
                Email = email,
                Role = role
            };
        }

        return null;
    }
}
