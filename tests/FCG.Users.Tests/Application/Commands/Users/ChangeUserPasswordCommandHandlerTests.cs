using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class ChangeUserPasswordCommandHandlerTests
{
    [Fact(DisplayName = "Validando alteração de senha com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ChangePassword_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash-antigo", UserProfile.User);

        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var passwordPolicy = new Mock<IPasswordPolicy>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwordHasher.Setup(x => x.Hash("NovaSenha@123"))
            .Returns("hash-novo");

        var handler = new ChangeUserPasswordCommandHandler(
            repository.Object,
            passwordHasher.Object,
            passwordPolicy.Object);

        await handler.HandleAsync(new ChangeUserPasswordCommand
        {
            Id = user.Id,
            NewPassword = "NovaSenha@123"
        });

        Assert.Equal("hash-novo", user.PasswordHash);

        passwordPolicy.Verify(x => x.Validate("NovaSenha@123"), Times.Once);
        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Validando alteração de senha para usuário inexistente")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ChangePassword_User_Not_Found()
    {
        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var passwordPolicy = new Mock<IPasswordPolicy>();

        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ChangeUserPasswordCommandHandler(
            repository.Object,
            passwordHasher.Object,
            passwordPolicy.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ChangeUserPasswordCommand
            {
                Id = Guid.NewGuid(),
                NewPassword = "NovaSenha@123"
            }));

        Assert.Equal("User not found.", result.Message);
    }
}