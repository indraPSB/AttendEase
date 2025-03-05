using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal static class UserService
{
    public static async Task<IEnumerable<User>?> GetUsers<T>(ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM user;
            return await context.Users.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUsers with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<User?> GetUser<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM user WHERE id = @id;
            return await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUser(Id) with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<User?> GetUser<T>(string email, string password, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM user WHERE email = @email AND password = @password;
            return await context.Users.SingleOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper() && u.Password == password, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUser(Email, Password) with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<bool> AddUser<T>(User user, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // INSERT INTO user (id, name, password, email, role) VALUES (@id, @name, @password, @email, @role);
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB AddUser with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> UpdateUser<T>(User user, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // UPDATE user SET name = @name, password = @password, email = @email, role = @role WHERE id = @id;
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB UpdateUser with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteUser<T>(Guid id, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM user WHERE id = @id;
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (user is not null)
            {
                // DELETE FROM user WHERE id = @id;
                context.Users.Remove(user);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteUser with message, '{message}'.", ex.Message);
        }

        return false;
    }

    public static async Task<bool> DeleteUsers<T>(IEnumerable<Guid> ids, ILogger<T> logger, AttendEaseDbContext context, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        try
        {
            // SELECT * FROM user WHERE id IN @ids;
            IEnumerable<User> users = await context.Users.Where(u => ids.Contains(u.Id)).ToListAsync(cancellationToken);

            if (users.Any())
            {
                // DELETE FROM user WHERE id IN @ids;
                context.Users.RemoveRange(users);
                await context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB DeleteUsers with message, '{message}'.", ex.Message);
        }

        return false;
    }
}
