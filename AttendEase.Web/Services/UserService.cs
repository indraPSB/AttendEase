using AttendEase.DB.Contexts;
using AttendEase.DB.Models;
using AttendEase.Shared.Services;
using UserDBService = AttendEase.DB.Services.UserService;

namespace AttendEase.Web.Services;

internal class UserService(ILogger<UserService> logger, AttendEaseDbContext context) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly AttendEaseDbContext _context = context;

    public Task<IEnumerable<User>?> GetUsers(CancellationToken ct = default)
    {
        return UserDBService.GetUsers(_logger, _context, ct);
    }

    public Task<User?> GetUser(Guid id, CancellationToken ct = default)
    {
        return UserDBService.GetUser(_logger, _context, id, ct);
    }
}
