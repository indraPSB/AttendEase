namespace AttendEase.Shared.Services;

public interface IAuthService
{
    Task<string?> GetToken(CancellationToken cancellationToken = default);

    Task<bool> Login(LoginRequest request, CancellationToken cancellationToken = default);

    Task Logout(CancellationToken cancellationToken = default);
}

public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginResponse
{
    public string? Token { get; set; }
}
