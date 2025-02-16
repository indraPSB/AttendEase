using AttendEase.DB.Models;
using AttendEase.Shared.Models;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Authorization;

namespace AttendEase.Web.Endpoints;

internal static class UserEndpoint
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("api/users", GetUsers)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapGet("api/users/{id:guid}", GetUser)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        return app;
    }

    public static async Task<IResult> GetUsers(IUserService userService, CancellationToken cancellationToken)
    {
        IEnumerable<User>? users = await userService.GetUsers(cancellationToken);

        if (users is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(users);
    }

    public static async Task<IResult> GetUser(Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        User? user = await userService.GetUser(id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(user);
    }
}
