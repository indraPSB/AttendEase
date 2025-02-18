﻿using AttendEase.DB.Contexts;
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
            return await context.Users.SingleOrDefaultAsync(u => u.Email == email && u.Password == password, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in DB GetUser(Email, Password) with message, '{message}'.", ex.Message);
        }

        return null;
    }
}
