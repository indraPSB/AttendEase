using AttendEase.DB.Models;

namespace AttendEase.Shared.Services;

public interface IUserService
{
    Task<IEnumerable<User>?> GetUsers(CancellationToken ct = default);

    Task<User?> GetUser(Guid id, CancellationToken ct = default);
}
