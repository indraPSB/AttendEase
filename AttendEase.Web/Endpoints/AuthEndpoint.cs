using AttendEase.Shared.Services;
using AttendEase.Web.Services;

namespace AttendEase.Web.Endpoints;

internal static class AuthEndpoint
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("api/auth/login", Login);

        return app;
    }

    public static async Task<IResult> Login(LoginRequest request, IAuthService authService, CancellationToken cancellationToken)
    {
        if (request is { Email: not null, Password: not null } && authService is AuthService authWebService)
        {
            string? token = await authWebService.GenerateToken(request, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                LoginResponse response = new()
                {
                    Token = token
                };

                return Results.Ok(response);
            }
        }

        return Results.BadRequest();
    }
}
