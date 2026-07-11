using FCG.Users.Application.Commands.Auth;
using FCG.Users.Application.Commands.Auth.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Auth;

public class LoginCommandHandlerTests
{
    [Fact(DisplayName = "Validando login com sucesso")]
    [Trait("Categoria", "Application - Auth")]
    public async Task Login_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        repository.Setup(x => x.GetByEmailAsync("bruno@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwordHasher.Setup(x => x.Verify("Senha@123", "hash"))
            .Returns(true);

        tokenService.Setup(x => x.Generate(user))
            .Returns("token-jwt");

        var handler = new LoginCommandHandler(
            repository.Object,
            passwordHasher.Object,
            tokenService.Object);

        var result = await handler.HandleAsync(new LoginCommand
        {
            Email = "bruno@email.com",
            Password = "Senha@123"
        });

        Assert.Equal("token-jwt", result.Token);
    }

    [Fact(DisplayName = "Validando login com usuário inexistente")]
    [Trait("Categoria", "Application - Auth")]
    public async Task Login_User_Not_Found()
    {
        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        repository.Setup(x => x.GetByEmailAsync("bruno@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new LoginCommandHandler(
            repository.Object,
            passwordHasher.Object,
            tokenService.Object);

        var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.HandleAsync(new LoginCommand
            {
                Email = "bruno@email.com",
                Password = "Senha@123"
            }));

        Assert.Equal("Invalid email or password.", result.Message);
    }

    [Fact(DisplayName = "Validando login com senha inválida")]
    [Trait("Categoria", "Application - Auth")]
    public async Task Login_Invalid_Password()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenService = new Mock<ITokenService>();

        repository.Setup(x => x.GetByEmailAsync("bruno@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        passwordHasher.Setup(x => x.Verify("SenhaErrada", "hash"))
            .Returns(false);

        var handler = new LoginCommandHandler(
            repository.Object,
            passwordHasher.Object,
            tokenService.Object);

        var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.HandleAsync(new LoginCommand
            {
                Email = "bruno@email.com",
                Password = "SenhaErrada"
            }));

        Assert.Equal("Invalid email or password.", result.Message);
    }
}