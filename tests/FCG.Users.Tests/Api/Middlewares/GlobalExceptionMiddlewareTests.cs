using FCG.Users.Api.Correlation;
using FCG.Users.Api.Middlewares;
using FCG.Users.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace FCG.Users.Tests.Api.Middlewares;

public class GlobalExceptionMiddlewareTests
{
    [Fact(DisplayName = "Validando tratamento de DomainException")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task GlobalExceptionMiddleware_HandleDomainException()
    {
        var correlationId = Guid.NewGuid();

        var context = CreateHttpContext();
        var accessor = new CorrelationIdAccessor();
        accessor.Set(correlationId);

        var logger = new Mock<ILogger<GlobalExceptionMiddleware>>();

        var middleware = new GlobalExceptionMiddleware(
            _ => throw new DomainException("Erro de domínio."),
            logger.Object);

        await middleware.Invoke(context, accessor);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);

        var response = await ReadResponseBodyAsync(context);

        Assert.Contains("Erro de dom", response);
        Assert.Contains(correlationId.ToString(), response);
    }

    [Fact(DisplayName = "Validando tratamento de UnauthorizedAccessException")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task GlobalExceptionMiddleware_HandleUnauthorizedAccessException()
    {
        var correlationId = Guid.NewGuid();

        var context = CreateHttpContext();
        var accessor = new CorrelationIdAccessor();
        accessor.Set(correlationId);

        var logger = new Mock<ILogger<GlobalExceptionMiddleware>>();

        var middleware = new GlobalExceptionMiddleware(
            _ => throw new UnauthorizedAccessException("Acesso negado."),
            logger.Object);

        await middleware.Invoke(context, accessor);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status401Unauthorized);

        var response = await ReadResponseBodyAsync(context);

        Assert.Contains("Acesso negado.", response);
        Assert.Contains(correlationId.ToString(), response);
    }

    [Fact(DisplayName = "Validando tratamento de erro inesperado")]
    [Trait("Categoria", "API - Middlewares")]
    public async Task GlobalExceptionMiddleware_HandleUnexpectedException()
    {
        var correlationId = Guid.NewGuid();

        var context = CreateHttpContext();
        var accessor = new CorrelationIdAccessor();
        accessor.Set(correlationId);

        var logger = new Mock<ILogger<GlobalExceptionMiddleware>>();

        var middleware = new GlobalExceptionMiddleware(
            _ => throw new Exception("Erro interno."),
            logger.Object);

        await middleware.Invoke(context, accessor);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);

        var response = await ReadResponseBodyAsync(context);

        Assert.Contains("Unexpected error.", response);
        Assert.Contains(correlationId.ToString(), response);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static async Task<string> ReadResponseBodyAsync(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(context.Response.Body);

        return await reader.ReadToEndAsync();
    }
}

public static class HttpStatusAssertExtensions
{
    public static void ShouldBe(this int actual, int expected)
    {
        Assert.Equal(expected, actual);
    }
}