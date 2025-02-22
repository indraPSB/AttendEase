﻿using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using UserDBService = AttendEase.DB.Services.UserService;

namespace AttendEase.Web.Services;

internal class UserService(ILogger<UserService> logger, AttendEaseDbContext context) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<IEnumerable<User>?> GetUsers(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.GetUsers(_logger, _context, cancellationToken);
    }

    public Task<User?> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.GetUser(id, _logger, _context, cancellationToken);
    }

    public Task<bool> AddUser(User user, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.AddUser(user, _logger, _context, cancellationToken);
    }

    public Task<bool> UpdateUser(User user, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.UpdateUser(user, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.DeleteUser(id, _logger, _context, cancellationToken);
    }

    public Task<bool> DeleteUsers(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default)
        {
            cancellationToken = CancellationToken.None;
        }

        return UserDBService.DeleteUsers(ids, _logger, _context, cancellationToken);
    }
}
