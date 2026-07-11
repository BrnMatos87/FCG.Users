using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using Moq;

namespace FCG.Users.Tests.Application.Commands.Users;

public class ChangeUserProfileCommandHandlerTests
{
    [Fact(DisplayName = "Validando alteração de perfil com sucesso")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ChangeProfile_Success()
    {
        var user = User.Create("Bruno", "bruno@email.com", "hash", UserProfile.User);

        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new ChangeUserProfileCommandHandler(repository.Object);

        await handler.HandleAsync(new ChangeUserProfileCommand
        {
            Id = user.Id,
            Profile = UserProfile.Administrator
        });

        Assert.Equal(UserProfile.Administrator, user.Profile);

        repository.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Validando alteração de perfil para usuário inexistente")]
    [Trait("Categoria", "Application - Usuários")]
    public async Task ChangeProfile_User_Not_Found()
    {
        var repository = new Mock<IUserRepository>();

        repository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new ChangeUserProfileCommandHandler(repository.Object);

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ChangeUserProfileCommand
            {
                Id = Guid.NewGuid(),
                Profile = UserProfile.Administrator
            }));

        Assert.Equal("Usuário não encontrado.", result.Message);
    }
}