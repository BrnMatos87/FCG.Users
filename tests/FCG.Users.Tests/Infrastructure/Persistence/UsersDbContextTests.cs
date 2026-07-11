using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using FCG.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Tests.Infrastructure.Persistence;

public class UsersDbContextTests
{
    [Fact(DisplayName = "Validando persistência de usuário")]
    [Trait("Categoria", "Infrastructure - Persistence")]
    public async Task DbContext_Save_User()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new UsersDbContext(options);

        var user = User.Create(
            "Bruno",
            "bruno@email.com",
            "hash",
            UserProfile.User);

        context.Users.Add(user);

        await context.SaveChangesAsync();

        Assert.Single(context.Users);
    }
}