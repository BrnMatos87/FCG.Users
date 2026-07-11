using FCG.Users.Api.Correlation;
using FCG.Users.Application.Contracts;

namespace FCG.Users.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        return services;
    }
}