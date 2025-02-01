using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserDBService = AttendEase.DB.Services.UserService;

namespace AttendEase.Web.Services;

internal class AuthService(ILogger<AuthService> logger, AttendEaseDbContext context, IConfiguration configuration, ProtectedLocalStorage protectedLocalStorage) : IAuthService
{
    const string Key = "jwt_token";

    private readonly ILogger<AuthService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;
    private readonly IConfiguration _config = configuration;
    private readonly ProtectedLocalStorage _protectedLocalStorage = protectedLocalStorage;

    public async Task<string?> GenerateToken(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            if (request is { Email: not null, Password: not null })
            {
                User? user = await UserDBService.GetUser(request.Email, request.Password, _logger, _context, cancellationToken);
                if (user is { Email: not null, Role: not null })
                {
                    JwtSecurityTokenHandler tokenHandler = new();
                    byte[] key = Encoding.UTF8.GetBytes(_config["JwtBearer:IssuerSigningKey"] ?? throw new InvalidOperationException("The signing key is missing from the configuration."));
                    SecurityTokenDescriptor tokenDescriptor = new()
                    {
                        Subject = new ClaimsIdentity([
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, user.Role)]),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = _config["JwtBearer:Issuer"] ?? throw new InvalidOperationException("The issuer is missing from the configuration."),
                        Audience = _config["JwtBearer:Audience"] ?? throw new InvalidOperationException("The audience is missing from the configuration."),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

                    return tokenHandler.WriteToken(token);
                }
                else
                {
                    _logger.LogWarning("User is null in Web AuthService.GenerateToken.");
                }
            }
            else
            {
                _logger.LogWarning("Login request is null in Web AuthService.GenerateToken.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Web AuthService.GenerateToken with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public async Task<string?> GetToken(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        ProtectedBrowserStorageResult<string> token = await _protectedLocalStorage.GetAsync<string>(Key);
        return token.Value;
    }

    public async Task<bool> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            string? token = await GenerateToken(request, cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("User is null in Web AuthService.Login.");
            }
            else
            {
                await _protectedLocalStorage.SetAsync(Key, token);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Web AuthService.Login with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public async Task Logout(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        await _protectedLocalStorage.DeleteAsync(Key);
    }
}
