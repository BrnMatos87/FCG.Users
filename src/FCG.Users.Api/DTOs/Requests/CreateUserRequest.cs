using FCG.Users.Domain.Enums;

namespace FCG.Users.Api.DTOs.Requests;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserProfile Profile { get; set; } = UserProfile.User;
}