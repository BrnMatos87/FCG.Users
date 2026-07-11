using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Tests.Infrastructure.Repositories;

public class UserRepositoryTests
{
    [Fact(DisplayName = "Validando criação de usuário no repositório")]
    [Trait("Categoria", "Infrastructure - Repository")]
    public async Task Repository_Create_Success()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new UsersDbContext(options);

        var repository = new UserRepository(context);

        var user = User.Create(
            "Bruno",
            "bruno@email.com",
            "hash",
            UserProfile.User);

        await repository.CreateAsync(user);

        var saved = await repository.GetByIdAsync(user.Id);

        Assert.NotNull(saved);
        Assert.Equal("Bruno", saved.Name);
    }

    [Fact(DisplayName = "Validando busca por e-mail")]
    [Trait("Categoria", "Infrastructure - Repository")]
    public async Task Repository_GetByEmail()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new UsersDbContext(options);

        var repository = new UserRepository(context);

        var user = User.Create(
            "Bruno",
            "bruno@email.com",
            "hash",
            UserProfile.User);

        await repository.CreateAsync(user);

        var result = await repository.GetByEmailAsync("bruno@email.com");

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact(DisplayName = "Validando busca de todos os usuários")]
    [Trait("Categoria", "Infrastructure - Repository")]
    public async Task Repository_GetAll()
    {
        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new UsersDbContext(options);

        var repository = new UserRepository(context);

        await repository.CreateAsync(
            User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User));

        await repository.CreateAsync(
            User.Create("Admin", "admin@email.com", "hash", UserProfile.Administrator));

        var result = await repository.GetAllAsync();

        Assert.Equal(2, result.Count());
    }
}