namespace FCG.Users.Application.Commands.Users;

public class UpdateUserCommand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}