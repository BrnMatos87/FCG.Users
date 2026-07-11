using FCG.Users.Api.Middlewares;

namespace FCG.Users.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApplicationMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }
}