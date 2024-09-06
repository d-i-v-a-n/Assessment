using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default);
    Task<User> GetUserByIdAsync(string id, CancellationToken ct = default);
}
