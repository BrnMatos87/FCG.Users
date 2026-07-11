using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class ChangeUserProfileCommandHandler : ICommandHandlerVoid<ChangeUserProfileCommand>
{
    private readonly IUserRepository _userRepository;

    public ChangeUserProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task HandleAsync(ChangeUserProfileCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, ct);

        if (user is null)
            throw new InvalidOperationException("Usuário não encontrado.");

        user.ChangeProfile(command.Profile);

        await _userRepository.UpdateAsync(user, ct);
    }
}