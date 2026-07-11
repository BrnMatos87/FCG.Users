using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class UpdateUserCommandHandler : ICommandHandlerVoid<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(UpdateUserCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, ct);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        user.Update(command.Name, command.Email);

        await _userRepository.UpdateAsync(user, ct);
    }
}