using FCG.BuildingBlocks.Events;
using FCG.Users.Application.Contracts;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Infrastructure.Messaging;

public class UserEventPublisher : IUserEventPublisher
{
    private const string UserCreatedRoute = "api/notifications/user-created";
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserEventPublisher> _logger;

    public UserEventPublisher(
        HttpClient httpClient,
        ILogger<UserEventPublisher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task PublishUserCreatedAsync(
        UserCreatedEvent message,
        CancellationToken ct = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            UserCreatedRoute,
            message,
            ct);

        if (response.IsSuccessStatusCode)
            return;

        _logger.LogError(
            "Falha ao chamar Notifications para o usuário {UserId}. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}",
            message.UserId,
            (int)response.StatusCode,
            message.CorrelationId);

        throw new HttpRequestException(
            $"Notifications retornou HTTP {(int)response.StatusCode} para UserCreated.",
            null,
            response.StatusCode);
    }
}
