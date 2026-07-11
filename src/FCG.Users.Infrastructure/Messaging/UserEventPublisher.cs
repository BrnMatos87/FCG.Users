using FCG.BuildingBlocks.Events;
using FCG.Users.Application.Contracts;
using MassTransit;

namespace FCG.Users.Infrastructure.Messaging;

public class UserEventPublisher : IUserEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public UserEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishUserCreatedAsync(
        UserCreatedEvent message,
        CancellationToken ct = default)
    {
        await _publishEndpoint.Publish(message, ct);
    }
}