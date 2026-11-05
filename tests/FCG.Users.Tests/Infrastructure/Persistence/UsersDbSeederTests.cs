using FCG.Users.Domain.Enums;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Tests.Infrastructure.Persistence;

public class UsersDbSeederTests
{
    [Fact(DisplayName = "Criando administrador com credenciais configuradas")]
    [Trait("Categoria", "Infrastructure - Persistence")]
    public async Task SeedAsync_ConfiguredCredentials_ShouldCreateAdministrator()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new UsersDbContext(options);

        const string email = "configured-admin@fcg.local";
        const string password = "Configured@123";

        await UsersDbSeeder.SeedAsync(context, email, password);

        var admin = await context.Users.SingleAsync();
        Assert.Equal(email, admin.Email);
        Assert.Equal(UserProfile.Administrator, admin.Profile);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash));
    }

    [Fact(DisplayName = "Não duplicando administrador durante novo startup")]
    [Trait("Categoria", "Infrastructure - Persistence")]
    public async Task SeedAsync_ExistingAdministrator_ShouldNotDuplicate()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new UsersDbContext(options);

        await UsersDbSeeder.SeedAsync(
            context,
            "configured-admin@fcg.local",
            "Configured@123");
        await UsersDbSeeder.SeedAsync(
            context,
            "configured-admin@fcg.local",
            "Configured@123");

        Assert.Equal(1, await context.Users.CountAsync());
    }
}
