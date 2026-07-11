using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.Persistence.Seeders;

public static class UsersDbSeeder
{
    public static async Task SeedAsync(UsersDbContext context)
    {
        const string adminEmail = "admin@fcg.com";

        var adminExists = await context.Users
            .AnyAsync(x => x.Email == adminEmail);

        if (adminExists)
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

        var admin = User.Create(
            "Administrator",
            adminEmail,
            passwordHash,
            UserProfile.Administrator);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}