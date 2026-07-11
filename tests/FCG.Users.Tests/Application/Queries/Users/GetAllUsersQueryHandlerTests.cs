using FCG.Users.Application.Contracts;
using FCG.Users.Application.Queries.Users;
using FCG.Users.Application.Queries.Users.Handlers;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Queries.Users;

public class GetAllUsersQueryHandlerTests
{
    [Fact(DisplayName = "Validando consulta de todos os usuários")]
    [Trait("Categoria", "Application - Queries")]
    public async Task GetAllUsers_Success()
    {
        var users = new List<User>
        {
            User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User),
            User.Create("Admin", "admin@email.com", "hash", UserProfile.Administrator)
        };

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var handler = new GetAllUsersQueryHandler(repository.Object);

        var result = await handler.HandleAsync(new GetAllUsersQuery());

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, x =>
            x.Name == "Bruno" &&
            x.Email == "bruno@email.com" &&
            x.Profile == UserProfile.User);

        Assert.Contains(result, x =>
            x.Name == "Admin" &&
            x.Email == "admin@email.com" &&
            x.Profile == UserProfile.Administrator);
    }

    [Fact(DisplayName = "Validando consulta de todos os usuários sem registros")]
    [Trait("Categoria", "Application - Queries")]
    public async Task GetAllUsers_Empty()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var handler = new GetAllUsersQueryHandler(repository.Object);

        var result = await handler.HandleAsync(new GetAllUsersQuery());

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}