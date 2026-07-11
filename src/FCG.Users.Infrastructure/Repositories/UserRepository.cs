using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<IList<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalizedEmail = email.Trim().ToLower();

        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, ct);
    }

    public async Task CreateAsync(User entity, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CreateManyAsync(IEnumerable<User> entities, CancellationToken ct = default)
    {
        await _context.Users.AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User entity, CancellationToken ct = default)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(id, ct);

        if (user is null)
            return;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(ct);
    }
}