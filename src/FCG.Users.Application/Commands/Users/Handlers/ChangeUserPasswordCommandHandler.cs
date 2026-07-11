using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class ChangeUserPasswordCommandHandler : ICommandHandlerVoid<ChangeUserPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicy _passwordPolicy;

    public ChangeUserPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordPolicy passwordPolicy)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordPolicy = passwordPolicy;
    }

    public async Task HandleAsync(ChangeUserPasswordCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, ct);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        _passwordPolicy.Validate(command.NewPassword);

        var passwordHash = _passwordHasher.Hash(command.NewPassword);

        user.ChangePassword(passwordHash);

        await _userRepository.UpdateAsync(user, ct);
    }
}