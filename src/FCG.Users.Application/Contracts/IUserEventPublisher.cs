using FCG.BuildingBlocks.Events;

namespace FCG.Users.Application.Contracts;

public interface IUserEventPublisher
{
    Task PublishUserCreatedAsync(UserCreatedEvent message, CancellationToken ct = default);
}