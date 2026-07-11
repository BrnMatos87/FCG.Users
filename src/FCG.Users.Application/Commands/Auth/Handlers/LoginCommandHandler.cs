using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;
using FCG.Users.Application.Responses;

namespace FCG.Users.Application.Commands.Auth.Handlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> HandleAsync(LoginCommand command, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(command.Email, ct);

        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new LoginResponse
        {
            Token = _tokenService.Generate(user)
        };
    }
}