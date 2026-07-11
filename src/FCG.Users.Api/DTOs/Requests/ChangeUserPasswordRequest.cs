namespace FCG.Users.Api.DTOs.Requests;

public class ChangeUserPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}