using FCG.BuildingBlocks.Enums;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class ActivateUserCommandHandlerTests
{
    [Fact(DisplayName = "Validando ativação de usuário com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ActivateUser_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);
        user.Inactivate();

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ActivateUserCommandHandler(repository.Object);

        await handler.HandleAsync(new ActivateUserCommand { Id = user.Id });

        Assert.Equal(StatusType.Active, user.Status);

        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Validando ativação de usuário inexistente")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ActivateUser_Not_Found()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ActivateUserCommandHandler(repository.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ActivateUserCommand { Id = Guid.NewGuid() }));

        Assert.Equal("User not found.", result.Message);
    }
}