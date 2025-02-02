using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AttendEase.Shared.Providers;

public class AttendEaseAuthenticationStateProvider(ILogger<AttendEaseAuthenticationStateProvider> logger, IAuthService authService) : AuthenticationStateProvider
{
    private readonly ILogger<AttendEaseAuthenticationStateProvider> _logger = logger;
    private readonly IAuthService _authService = authService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal? principal = null;

        try
        {
            User? user = await _authService.GetUser();

            if (user is { Name: not null, Email: not null, Role: not null } && user.Id != Guid.Empty)
            {
                principal = new ClaimsPrincipal(
                    new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)], "jwt"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state with message, '{message}.", ex.Message);
        }

        return new AuthenticationState(principal ?? new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
