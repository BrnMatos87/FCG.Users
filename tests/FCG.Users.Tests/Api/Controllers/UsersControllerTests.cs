using FCG.BuildingBlocks.Enums;
using FCG.Users.Api.Controllers;
using FCG.Users.Api.DTOs.Requests;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Abstractions.Queries;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Queries.Users;
using FCG.Users.Application.Responses;
using FCG.Users.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FCG.Users.Tests.Api.Controllers;

public class UsersControllerTests
{
    private readonly Mock<ICommandHandler<CreateUserCommand, Guid>> _createUserCommandHandler = new();
    private readonly Mock<ICommandHandlerVoid<UpdateUserCommand>> _updateUserCommandHandler = new();
    private readonly Mock<ICommandHandlerVoid<ChangeUserPasswordCommand>> _changeUserPasswordCommandHandler = new();
    private readonly Mock<ICommandHandlerVoid<ChangeUserProfileCommand>> _changeUserProfileCommandHandler = new();
    private readonly Mock<ICommandHandlerVoid<ActivateUserCommand>> _activateUserCommandHandler = new();
    private readonly Mock<ICommandHandlerVoid<InactivateUserCommand>> _inactivateUserCommandHandler = new();
    private readonly Mock<IQueryHandler<GetAllUsersQuery, IList<UserResponse>>> _getAllUsersQueryHandler = new();
    private readonly Mock<IQueryHandler<GetUserByIdQuery, UserResponse?>> _getUserByIdQueryHandler = new();

    [Fact(DisplayName = "Validando cadastro público de usuário")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_Register_Success()
    {
        var userId = Guid.NewGuid();

        _createUserCommandHandler.Setup(x => x.HandleAsync(
                It.IsAny<CreateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var controller = CreateController();

        var result = await controller.Register(new CreateUserRegisterRequest
        {
            Name = "Bruno",
            Email = "bruno@email.com",
            Password = "Senha@123"
        }, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(UsersController.GetById), createdResult.ActionName);

        _createUserCommandHandler.Verify(x => x.HandleAsync(
            It.Is<CreateUserCommand>(c =>
                c.Name == "Bruno" &&
                c.Email == "bruno@email.com" &&
                c.Password == "Senha@123" &&
                c.Profile == UserProfile.User),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando criação administrativa de usuário")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_Create_Success()
    {
        var userId = Guid.NewGuid();

        _createUserCommandHandler.Setup(x => x.HandleAsync(
                It.IsAny<CreateUserCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        var controller = CreateController();

        var result = await controller.Create(new CreateUserRequest
        {
            Name = "Admin",
            Email = "admin@email.com",
            Password = "Admin@123",
            Profile = UserProfile.Administrator
        }, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);

        Assert.Equal(nameof(UsersController.GetById), createdResult.ActionName);

        _createUserCommandHandler.Verify(x => x.HandleAsync(
            It.Is<CreateUserCommand>(c =>
                c.Name == "Admin" &&
                c.Email == "admin@email.com" &&
                c.Password == "Admin@123" &&
                c.Profile == UserProfile.Administrator),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando consulta de todos os usuários")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_GetAll_Success()
    {
        var users = new List<UserResponse>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Bruno",
                Email = "bruno@email.com",
                Profile = UserProfile.User,
                Status = StatusType.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        _getAllUsersQueryHandler.Setup(x => x.HandleAsync(
                It.IsAny<GetAllUsersQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var controller = CreateController();

        var result = await controller.GetAll(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsAssignableFrom<IList<UserResponse>>(okResult.Value);

        Assert.Single(response);
    }

    [Fact(DisplayName = "Validando consulta de usuário por id")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_GetById_Success()
    {
        var userId = Guid.NewGuid();

        var user = new UserResponse
        {
            Id = userId,
            Name = "Bruno",
            Email = "bruno@email.com",
            Profile = UserProfile.User,
            Status = StatusType.Active,
            CreatedAt = DateTime.UtcNow
        };

        _getUserByIdQueryHandler.Setup(x => x.HandleAsync(
                It.Is<GetUserByIdQuery>(q => q.Id == userId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var controller = CreateController();

        var result = await controller.GetById(userId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(userId, response.Id);
    }

    [Fact(DisplayName = "Validando consulta de usuário inexistente")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_GetById_NotFound()
    {
        var userId = Guid.NewGuid();

        _getUserByIdQueryHandler.Setup(x => x.HandleAsync(
                It.IsAny<GetUserByIdQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserResponse?)null);

        var controller = CreateController();

        var result = await controller.GetById(userId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact(DisplayName = "Validando atualização de usuário")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_Update_Success()
    {
        var userId = Guid.NewGuid();

        var controller = CreateController();

        var result = await controller.Update(userId, new UpdateUserRequest
        {
            Name = "Bruno Silva",
            Email = "bruno.silva@email.com"
        }, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _updateUserCommandHandler.Verify(x => x.HandleAsync(
            It.Is<UpdateUserCommand>(c =>
                c.Id == userId &&
                c.Name == "Bruno Silva" &&
                c.Email == "bruno.silva@email.com"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando alteração de senha")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_ChangePassword_Success()
    {
        var userId = Guid.NewGuid();

        var controller = CreateController();

        var result = await controller.ChangePassword(userId, new ChangeUserPasswordRequest
        {
            NewPassword = "NovaSenha@123"
        }, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _changeUserPasswordCommandHandler.Verify(x => x.HandleAsync(
            It.Is<ChangeUserPasswordCommand>(c =>
                c.Id == userId &&
                c.NewPassword == "NovaSenha@123"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando alteração de perfil")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_ChangeProfile_Success()
    {
        var userId = Guid.NewGuid();

        var controller = CreateController();

        var result = await controller.ChangeProfile(userId, new ChangeUserProfileRequest
        {
            Profile = UserProfile.Administrator
        }, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _changeUserProfileCommandHandler.Verify(x => x.HandleAsync(
            It.Is<ChangeUserProfileCommand>(c =>
                c.Id == userId &&
                c.Profile == UserProfile.Administrator),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando ativação de usuário")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_Activate_Success()
    {
        var userId = Guid.NewGuid();

        var controller = CreateController();

        var result = await controller.Activate(userId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _activateUserCommandHandler.Verify(x => x.HandleAsync(
            It.Is<ActivateUserCommand>(c => c.Id == userId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Validando inativação de usuário")]
    [Trait("Categoria", "API - UsersController")]
    public async Task UsersController_Inactivate_Success()
    {
        var userId = Guid.NewGuid();

        var controller = CreateController();

        var result = await controller.Inactivate(userId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        _inactivateUserCommandHandler.Verify(x => x.HandleAsync(
            It.Is<InactivateUserCommand>(c => c.Id == userId),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private UsersController CreateController()
    {
        return new UsersController(
            _createUserCommandHandler.Object,
            _updateUserCommandHandler.Object,
            _changeUserPasswordCommandHandler.Object,
            _changeUserProfileCommandHandler.Object,
            _activateUserCommandHandler.Object,
            _inactivateUserCommandHandler.Object,
            _getAllUsersQueryHandler.Object,
            _getUserByIdQueryHandler.Object);
    }
}