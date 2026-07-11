using FCG.Users.Api.DTOs.Requests;
using FCG.Users.Api.Mappers;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Abstractions.Queries;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Queries.Users;
using FCG.Users.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly ICommandHandler<CreateUserCommand, Guid> _createUserCommandHandler;
    private readonly ICommandHandlerVoid<UpdateUserCommand> _updateUserCommandHandler;
    private readonly ICommandHandlerVoid<ChangeUserPasswordCommand> _changeUserPasswordCommandHandler;
    private readonly ICommandHandlerVoid<ChangeUserProfileCommand> _changeUserProfileCommandHandler;
    private readonly ICommandHandlerVoid<ActivateUserCommand> _activateUserCommandHandler;
    private readonly ICommandHandlerVoid<InactivateUserCommand> _inactivateUserCommandHandler;
    private readonly IQueryHandler<GetAllUsersQuery, IList<UserResponse>> _getAllUsersQueryHandler;
    private readonly IQueryHandler<GetUserByIdQuery, UserResponse?> _getUserByIdQueryHandler;

    public UsersController(
        ICommandHandler<CreateUserCommand, Guid> createUserCommandHandler,
        ICommandHandlerVoid<UpdateUserCommand> updateUserCommandHandler,
        ICommandHandlerVoid<ChangeUserPasswordCommand> changeUserPasswordCommandHandler,
        ICommandHandlerVoid<ChangeUserProfileCommand> changeUserProfileCommandHandler,
        ICommandHandlerVoid<ActivateUserCommand> activateUserCommandHandler,
        ICommandHandlerVoid<InactivateUserCommand> inactivateUserCommandHandler,
        IQueryHandler<GetAllUsersQuery, IList<UserResponse>> getAllUsersQueryHandler,
        IQueryHandler<GetUserByIdQuery, UserResponse?> getUserByIdQueryHandler)
    {
        _createUserCommandHandler = createUserCommandHandler;
        _updateUserCommandHandler = updateUserCommandHandler;
        _changeUserPasswordCommandHandler = changeUserPasswordCommandHandler;
        _changeUserProfileCommandHandler = changeUserProfileCommandHandler;
        _activateUserCommandHandler = activateUserCommandHandler;
        _inactivateUserCommandHandler = inactivateUserCommandHandler;
        _getAllUsersQueryHandler = getAllUsersQueryHandler;
        _getUserByIdQueryHandler = getUserByIdQueryHandler;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] CreateUserRegisterRequest request,
        CancellationToken ct)
    {
        var userId = await _createUserCommandHandler.HandleAsync(
            request.ToCommand(),
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            new { id = userId });
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var userId = await _createUserCommandHandler.HandleAsync(
            request.ToCommand(),
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = userId },
            new { id = userId });
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _getAllUsersQueryHandler.HandleAsync(
            new GetAllUsersQuery(),
            ct);

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await _getUserByIdQueryHandler.HandleAsync(
            new GetUserByIdQuery { Id = id },
            ct);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct)
    {
        await _updateUserCommandHandler.HandleAsync(
            request.ToCommand(id),
            ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        Guid id,
        [FromBody] ChangeUserPasswordRequest request,
        CancellationToken ct)
    {
        await _changeUserPasswordCommandHandler.HandleAsync(
            request.ToCommand(id),
            ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/profile")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ChangeProfile(
        Guid id,
        [FromBody] ChangeUserProfileRequest request,
        CancellationToken ct)
    {
        await _changeUserProfileCommandHandler.HandleAsync(
            request.ToCommand(id),
            ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
    {
        await _activateUserCommandHandler.HandleAsync(
            new ActivateUserCommand { Id = id },
            ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inactivate")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Inactivate(Guid id, CancellationToken ct)
    {
        await _inactivateUserCommandHandler.HandleAsync(
            new InactivateUserCommand { Id = id },
            ct);

        return NoContent();
    }
}