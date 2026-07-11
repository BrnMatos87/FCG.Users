using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class UpdateUserCommandHandlerTests
{
    [Fact(DisplayName = "Validando atualização de usuário com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task UpdateUser_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new UpdateUserCommandHandler(repository.Object);

        await handler.HandleAsync(new UpdateUserCommand
        {
            Id = user.Id,
            Name = "Bruno Silva",
            Email = "bruno.silva@email.com"
        });

        Assert.Equal("Bruno Silva", user.Name);
        Assert.Equal("bruno.silva@email.com", user.Email);

        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Validando atualização de usuário inexistente")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task UpdateUser_Not_Found()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new UpdateUserCommandHandler(repository.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new UpdateUserCommand
            {
                Id = Guid.NewGuid(),
                Name = "Bruno Silva",
                Email = "bruno.silva@email.com"
            }));

        Assert.Equal("User not found.", result.Message);
    }
}