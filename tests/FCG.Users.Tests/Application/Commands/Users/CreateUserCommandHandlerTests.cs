using FCG.BuildingBlocks.Events;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class CreateUserCommandHandlerTests
{
    [Fact(DisplayName = "Validando criação de usuário com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task CreateUser_Success()
    {
        var correlationId = Guid.NewGuid();

        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var eventPublisher = new Mock<IUserEventPublisher>();
        var passwordPolicy = new Mock<IPasswordPolicy>();
        var correlationAccessor = new Mock<ICorrelationIdAccessor>();

        repository.Setup(x => x.GetByEmailAsync("bruno@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        passwordHasher.Setup(x => x.Hash("Senha@123"))
            .Returns("hash-da-senha");

        correlationAccessor.Setup(x => x.Get())
            .Returns(correlationId);

        var handler = new CreateUserCommandHandler(
            repository.Object,
            passwordHasher.Object,
            eventPublisher.Object,
            correlationAccessor.Object,
            passwordPolicy.Object);

        var userId = await handler.HandleAsync(new CreateUserCommand
        {
            Name = "Bruno",
            Email = "bruno@email.com",
            Password = "Senha@123",
            Profile = UserProfile.User
        });

        Assert.NotEqual(Guid.Empty, userId);

        passwordPolicy.Verify(x => x.Validate("Senha@123"), Times.Once);

        repository.Verify(x => x.CreateAsync(
            It.Is<User>(u =>
                u.Name == "Bruno" &&
                u.Email == "bruno@email.com" &&
                u.PasswordHash == "hash-da-senha" &&
                u.Profile == UserProfile.User),
            It.IsAny<CancellationToken>()),
            Times.Once);

        eventPublisher.Verify(x => x.PublishUserCreatedAsync(
            It.Is<UserCreatedEvent>(e =>
                e.UserId == userId &&
                e.Name == "Bruno" &&
                e.Email == "bruno@email.com" &&
                e.CorrelationId == correlationId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando criação de usuário com e-mail já cadastrado")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task CreateUser_Email_Already_Registered()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var eventPublisher = new Mock<IUserEventPublisher>();
        var passwordPolicy = new Mock<IPasswordPolicy>();
        var correlationAccessor = new Mock<ICorrelationIdAccessor>();

        repository.Setup(x => x.GetByEmailAsync("bruno@email.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new CreateUserCommandHandler(
            repository.Object,
            passwordHasher.Object,
            eventPublisher.Object,
            correlationAccessor.Object,
            passwordPolicy.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new CreateUserCommand
            {
                Name = "Bruno",
                Email = "bruno@email.com",
                Password = "Senha@123",
                Profile = UserProfile.User
            }));

        Assert.Equal("E-mail já registrado.", result.Message);

        repository.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        eventPublisher.Verify(x => x.PublishUserCreatedAsync(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}