using FCG.BuildingBlocks.Enums;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class InactivateUserCommandHandlerTests
{
    [Fact(DisplayName = "Validando inativação de usuário com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task InactivateUser_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new InactivateUserCommandHandler(repository.Object);

        await handler.HandleAsync(new InactivateUserCommand { Id = user.Id });

        Assert.Equal(StatusType.Inactive, user.Status);

        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Validando inativação de usuário inexistente")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task InactivateUser_Not_Found()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new InactivateUserCommandHandler(repository.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new InactivateUserCommand { Id = Guid.NewGuid() }));

        Assert.Equal("User not found.", result.Message);
    }
}