using FCG.Users.Api.DTOs.Requests;
using FCG.Users.Application.Commands.Auth;
using FCG.Users.Application.Commands.Users;
using FCG.Users.Domain.Enums;

namespace FCG.Users.Api.Mappers;

public static class UserMapper
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
    {
        return new CreateUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Profile = request.Profile
        };
    }

    public static CreateUserCommand ToCommand(this CreateUserRegisterRequest request)
    {
        return new CreateUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Profile = UserProfile.User
        };
    }

    public static LoginCommand ToCommand(this LoginRequest request)
    {
        return new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };
    }

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid id)
    {
        return new UpdateUserCommand
        {
            Id = id,
            Name = request.Name,
            Email = request.Email
        };
    }

    public static ChangeUserPasswordCommand ToCommand(this ChangeUserPasswordRequest request, Guid id)
    {
        return new ChangeUserPasswordCommand
        {
            Id = id,
            NewPassword = request.NewPassword
        };
    }

    public static ChangeUserProfileCommand ToCommand(this ChangeUserProfileRequest request, Guid id)
    {
        return new ChangeUserProfileCommand
        {
            Id = id,
            Profile = request.Profile
        };
    }
}