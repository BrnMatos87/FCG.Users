using FCG.Users.Api.Correlation;
using FCG.Users.Api.Middlewares;
using Microsoft.AspNetCore.Http;

namespace FCG.Users.Tests.Api.Middlewares;

public class CorrelationMiddlewareTests
{
    [Fact(DisplayName = "Validando criação de correlation id quando não informado")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task CorrelationMiddleware_CreateCorrelationId_WhenHeaderDoesNotExist()
    {
        var accessor = new CorrelationIdAccessor();

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        await middleware.Invoke(context, accessor);

        var correlationId = accessor.Get();

        Assert.NotEqual(Guid.Empty, correlationId);
        Assert.Equal(correlationId.ToString(), context.Request.Headers["x-correlation-id"]);
    }

    [Fact(DisplayName = "Validando reutilização de correlation id informado no header")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task CorrelationMiddleware_UseExistingCorrelationId_WhenHeaderExists()
    {
        var existingCorrelationId = Guid.NewGuid();
        var accessor = new CorrelationIdAccessor();

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["x-correlation-id"] = existingCorrelationId.ToString();

        var middleware = new CorrelationMiddleware(_ => Task.CompletedTask);

        await middleware.Invoke(context, accessor);

        Assert.Equal(existingCorrelationId, accessor.Get());
        Assert.Equal(existingCorrelationId.ToString(), context.Request.Headers["x-correlation-id"]);
    }
}