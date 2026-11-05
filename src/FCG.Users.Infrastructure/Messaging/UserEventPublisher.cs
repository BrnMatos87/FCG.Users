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
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                UserCreatedRoute,
                message,
                ct);

            if (response.IsSuccessStatusCode)
                return;

            _logger.LogWarning(
                "Notifications indisponível para o usuário {UserId}. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}",
                message.UserId,
                (int)response.StatusCode,
                message.CorrelationId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                ex,
                "Não foi possível acessar Notifications para o usuário {UserId}. CorrelationId: {CorrelationId}",
                message.UserId,
                message.CorrelationId);
        }
        catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
        {
            _logger.LogWarning(
                ex,
                "Timeout ao acessar Notifications para o usuário {UserId}. CorrelationId: {CorrelationId}",
                message.UserId,
                message.CorrelationId);
        }
    }
}
