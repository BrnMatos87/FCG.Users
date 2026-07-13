using FCG.Users.Application.Contracts;
using FCG.Users.Infrastructure.Messaging;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Repositories;
using FCG.Users.Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FCG.Users.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddJwt(services, configuration);
        AddRabbitMq(services, configuration);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserEventPublisher, UserEventPublisher>();
        services.AddScoped<IPasswordPolicy, PasswordPolicy>();

        return services;
    }

    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        });
    }

    private static void AddJwt(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.SecretKey),
                "Jwt:SecretKey não foi configurado.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Jwt:Issuer não foi configurado.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "Jwt:Audience não foi configurado.")
            .Validate(
                options => options.ExpirationMinutes > 0,
                "Jwt:ExpirationMinutes deve ser maior que zero.")
            .ValidateOnStart();
    }

    private static void AddRabbitMq(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Host),
                "RabbitMq:Host não foi configurado.")
            .Validate(
                options => options.Port > 0,
                "RabbitMq:Port deve ser maior que zero.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.VirtualHost),
                "RabbitMq:VirtualHost não foi configurado.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Username),
                "RabbitMq:Username não foi configurado.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Password),
                "RabbitMq:Password não foi configurado.")
            .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                cfg.Host(
                    options.Host,
                    options.Port,
                    options.VirtualHost,
                    hostConfiguration =>
                    {
                        hostConfiguration.Username(options.Username);
                        hostConfiguration.Password(options.Password);
                    });
            });
        });
    }
}