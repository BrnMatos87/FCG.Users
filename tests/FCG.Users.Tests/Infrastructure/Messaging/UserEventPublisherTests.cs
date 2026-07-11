using FCG.BuildingBlocks.Events;
using FCG.Users.Infrastructure.Messaging;
using MassTransit;
using Moq;

namespace FCG.Users.Tests.Infrastructure.Messaging;

public class UserEventPublisherTests
{
    [Fact(DisplayName = "Validando publicação do evento UserCreated")]
    [Trait("Categoria", "Infrastructure - Messaging")]
    public async Task PublishUserCreatedEvent_Success()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();

        var publisher = new UserEventPublisher(
            publishEndpoint.Object);

        var message = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Name = "Bruno",
            Email = "bruno@email.com",
            CreatedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };

        await publisher.PublishUserCreatedAsync(message);

        publishEndpoint.Verify(x =>
            x.Publish(
                message,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}