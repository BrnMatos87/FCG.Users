using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class InactivateUserCommandHandler : ICommandHandlerVoid<InactivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public InactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(InactivateUserCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, ct);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.Inactivate();

        await _userRepository.UpdateAsync(user, ct);
    }
}