namespace FCG.Users.Application.Commands.Users;

public class ChangeUserPasswordCommand
{
    public Guid Id { get; set; }

    public string NewPassword { get; set; } = string.Empty;
}