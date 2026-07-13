using FCG.Users.Api.Extensions;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Abstractions.Queries;
using FCG.Users.Application.Commands.Auth;
using FCG.Users.Application.Commands.Auth.Handlers;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Application.Commands.Users.Handlers;
using FCG.Users.Application.Queries.Users;
using FCG.Users.Application.Queries.Users.Handlers;
using FCG.Users.Application.Responses;
using FCG.Users.Infrastructure.Extensions;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var runningInContainer =
    string.Equals(
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

if (builder.Environment.IsDevelopment() && !runningInContainer)
{
    builder.Configuration.AddJsonFile(
        "appsettings.Local.json",
        optional: true,
        reloadOnChange: true);
}

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApiServices();

builder.Services.AddScoped<
    ICommandHandler<LoginCommand, LoginResponse>,
    LoginCommandHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateUserCommand, Guid>,
    CreateUserCommandHandler>();

builder.Services.AddScoped<
    ICommandHandlerVoid<UpdateUserCommand>,
    UpdateUserCommandHandler>();

builder.Services.AddScoped<
    ICommandHandlerVoid<ChangeUserPasswordCommand>,
    ChangeUserPasswordCommandHandler>();

builder.Services.AddScoped<
    ICommandHandlerVoid<ChangeUserProfileCommand>,
    ChangeUserProfileCommandHandler>();

builder.Services.AddScoped<
    ICommandHandlerVoid<ActivateUserCommand>,
    ActivateUserCommandHandler>();

builder.Services.AddScoped<
    ICommandHandlerVoid<InactivateUserCommand>,
    InactivateUserCommandHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetAllUsersQuery, IList<UserResponse>>,
    GetAllUsersQueryHandler>();

builder.Services.AddScoped<
    IQueryHandler<GetUserByIdQuery, UserResponse?>,
    GetUserByIdQueryHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<UsersDbContext>();

    await dbContext.Database.MigrateAsync();

    await UsersDbSeeder.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseApplicationMiddlewares();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();