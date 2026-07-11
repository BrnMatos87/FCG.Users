using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class ActivateUserCommandHandler : ICommandHandlerVoid<ActivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public ActivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(ActivateUserCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, ct);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.Activate();

        await _userRepository.UpdateAsync(user, ct);
    }
}