using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Persistence.Seeders;

public static class UsersDbSeeder
{
    public static async Task SeedAsync(
        UsersDbContext context,
        string adminEmail,
        string adminPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(adminEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(adminPassword);

        adminEmail = adminEmail.Trim().ToLowerInvariant();

        var adminExists = await context.Users
            .AnyAsync(x => x.Email == adminEmail);

        if (adminExists)
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

        var admin = User.Create(
            "Administrator",
            adminEmail,
            passwordHash,
            UserProfile.Administrator);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
