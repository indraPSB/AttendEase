using AttendEase.DB.Models;
using AttendEase.Shared.Services;

namespace AttendEase.Web.Endpoints;

internal static class UserEndpoint
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("api/users", GetUsers);

        app.MapGet("api/users/{id:guid}", GetUser);

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
