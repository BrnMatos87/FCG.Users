using FCG.BuildingBlocks.Enums;
using FCG.Users.Application.Contracts;
using FCG.Users.Application.Queries.Users;
using FCG.Users.Application.Queries.Users.Handlers;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Queries.Users;

public class GetUserByIdQueryHandlerTests
{
    [Fact(DisplayName = "Validando consulta de usuário por id")]
    [Trait("Categoria", "Application - Queries")]
    public async Task GetUserByIdSuccess()
    {
        var user = User.Create(
            "Bruno",
            "bruno@email.com",
            "hash",
            UserProfile.User);

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserByIdQueryHandler(repository.Object);

        var result = await handler.HandleAsync(new GetUserByIdQuery
        {
            Id = user.Id
        });

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal("Bruno", result.Name);
        Assert.Equal("bruno@email.com", result.Email);
        Assert.Equal(UserProfile.User, result.Profile);
        Assert.Equal(StatusType.Active, result.Status);
    }

    [Fact(DisplayName = "Validando consulta de usuário inexistente por id")]
    [Trait("Categoria", "Application - Queries")]
    public async Task GetUserById_NotFound()
    {
        var userId = Guid.NewGuid();

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByIdQueryHandler(repository.Object);

        var result = await handler.HandleAsync(new GetUserByIdQuery
        {
            Id = userId
        });

        Assert.Null(result);
    }
}