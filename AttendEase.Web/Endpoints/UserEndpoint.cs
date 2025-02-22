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

        app.MapPost("api/users", AddUser)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPut("api/users", UpdateUser)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapDelete("api/users/{id:guid}", DeleteUser)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Admin},{UserRole.Business}" });

        app.MapPost("api/users/delete", DeleteUsers)
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

    public static async Task<IResult> AddUser(User user, IUserService userService, CancellationToken cancellationToken)
    {
        if (await userService.AddUser(user, cancellationToken))
        {
            return Results.Created($"/api/users/{user.Id}", user);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> UpdateUser(User user, IUserService userService, CancellationToken cancellationToken)
    {
        if (await userService.UpdateUser(user, cancellationToken))
        {
            return Results.Ok(user);
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteUser(Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        if (await userService.DeleteUser(id, cancellationToken))
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }

    public static async Task<IResult> DeleteUsers(IEnumerable<Guid> ids, IUserService userService, CancellationToken cancellationToken)
    {
        if (await userService.DeleteUsers(ids, cancellationToken))
        {
            return Results.NoContent();
        }

        return Results.BadRequest();
    }
}
