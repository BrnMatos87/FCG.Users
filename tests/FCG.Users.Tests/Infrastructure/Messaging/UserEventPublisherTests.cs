using FCG.BuildingBlocks.Events;
using FCG.Users.Infrastructure.Messaging;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Infrastructure.Messaging;

public class UserEventPublisherTests
{
    [Fact(DisplayName = "Validando publicação do evento UserCreated")]
    [Trait("Categoria", "Infrastructure - Messaging")]
    public async Task PublishUserCreatedEvent_Success()
    {
        var handler = new RecordingHttpMessageHandler(HttpStatusCode.NoContent);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://notifications.test/")
        };
        var logger = new Mock<ILogger<UserEventPublisher>>();

        var publisher = new UserEventPublisher(
            httpClient,
            logger.Object);

        var message = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            Name = "Bruno",
            Email = "bruno@email.com",
            CreatedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };

        await publisher.PublishUserCreatedAsync(message);

        Assert.Equal(
            new Uri("https://notifications.test/api/notifications/user-created"),
            handler.Request?.RequestUri);
        Assert.Equal(HttpMethod.Post, handler.Request?.Method);

        var request = JsonSerializer.Deserialize<UserCreatedEvent>(
            handler.Body!,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(message.UserId, request?.UserId);
        Assert.Equal(message.CorrelationId, request?.CorrelationId);
    }

    [Fact(DisplayName = "Propagando falha HTTP do Notifications")]
    [Trait("Categoria", "Infrastructure - Messaging")]
    public async Task PublishUserCreatedEvent_NotificationFailure_ShouldThrow()
    {
        var httpClient = new HttpClient(
            new RecordingHttpMessageHandler(HttpStatusCode.InternalServerError))
        {
            BaseAddress = new Uri("https://notifications.test/")
        };
        var publisher = new UserEventPublisher(
            httpClient,
            Mock.Of<ILogger<UserEventPublisher>>());

        var message = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            publisher.PublishUserCreatedAsync(message));
    }

    private sealed class RecordingHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        public RecordingHttpMessageHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        public HttpRequestMessage? Request { get; private set; }

        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Request = request;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(_statusCode);
        }
    }
}
