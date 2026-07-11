using FCG.BuildingBlocks.Events;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Entities;

namespace FCG.Users.Application.Commands.Users.Handlers;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserEventPublisher _userEventPublisher;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly IPasswordPolicy _passwordPolicy;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUserEventPublisher userEventPublisher,
        ICorrelationIdAccessor correlationIdAccessor,
        IPasswordPolicy passwordPolicy)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userEventPublisher = userEventPublisher;
        _correlationIdAccessor = correlationIdAccessor;
        _passwordPolicy = passwordPolicy;
    }

    public async Task<Guid> HandleAsync(CreateUserCommand command, CancellationToken ct = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, ct);

        if (existingUser is not null)
            throw new InvalidOperationException("E-mail já registrado.");

        _passwordPolicy.Validate(command.Password);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = User.Create(
            command.Name,
            command.Email,
            passwordHash,
            command.Profile);

        await _userRepository.CreateAsync(user, ct);

        await _userEventPublisher.PublishUserCreatedAsync(
            new UserCreatedEvent
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                CorrelationId = _correlationIdAccessor.Get()
            },
            ct);

        return user.Id;
    }
}