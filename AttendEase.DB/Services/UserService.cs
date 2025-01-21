using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AttendEase.DB.Services;

internal static class UserService
{
    private static ILogger _logger;

    static UserService()
    {
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        _logger = loggerFactory.CreateLogger(nameof(UserService));
    }

    public static async Task<IEnumerable<User>?> GetUsers(AttendEaseDbContext context, CancellationToken ct = default)
    {
        try
        {
            return await context.Users.ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUsers with message, '{message}'.", ex.Message);
        }

        return null;
    }

    public static async Task<User?> GetUser(AttendEaseDbContext context, Guid id, CancellationToken ct = default)
    {
        try
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Id == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUser with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
