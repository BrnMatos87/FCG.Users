namespace FCG.Users.Api.DTOs.Requests;

public class CreateUserRegisterRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}