namespace FCG.Users.Api.DTOs.Requests;

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}