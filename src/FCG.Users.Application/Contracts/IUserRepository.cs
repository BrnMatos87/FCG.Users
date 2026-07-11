using FCG.Users.Domain.Entities;

namespace FCG.Users.Application.Contracts;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
}