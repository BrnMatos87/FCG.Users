using FCG.Users.Application.Contracts;
using FCG.Users.Infrastructure.Messaging;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Repositories;
using FCG.Users.Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FCG.Users.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)));

        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<RabbitMqOptions>(
            configuration.GetSection("RabbitMq"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserEventPublisher, UserEventPublisher>();
        services.AddScoped<IPasswordPolicy, PasswordPolicy>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqOptions = configuration
                    .GetSection("RabbitMq")
                    .Get<RabbitMqOptions>()!;

                cfg.Host(
                    rabbitMqOptions.Host,
                    (ushort)rabbitMqOptions.Port,
                    rabbitMqOptions.VirtualHost,
                    h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });
            });
        });

        return services;
    }
}