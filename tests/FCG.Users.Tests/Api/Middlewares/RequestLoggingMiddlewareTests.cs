using FCG.Users.Api.Correlation;
using FCG.Users.Api.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Api.Middlewares;

public class RequestLoggingMiddlewareTests
{
    [Fact(DisplayName = "Validando execução do middleware de log")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task RequestLoggingMiddleware_Invoke_Success()
    {
        var nextWasCalled = false;
        var correlationId = Guid.NewGuid();

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/users";
        context.Response.Body = new MemoryStream();

        var accessor = new CorrelationIdAccessor();
        accessor.Set(correlationId);

        var logger = new Mock<ILogger<RequestLoggingMiddleware>>();

        var middleware = new RequestLoggingMiddleware(
            _ =>
            {
                nextWasCalled = true;
                return Task.CompletedTask;
            },
            logger.Object);

        await middleware.Invoke(context, accessor);

        Assert.True(nextWasCalled);
    }
}