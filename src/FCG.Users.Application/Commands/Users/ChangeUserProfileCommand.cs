using FCG.Users.Domain.Enums;

namespace FCG.Users.Application.Commands.Users;

public class ChangeUserProfileCommand
{
    public Guid Id { get; set; }

    public UserProfile Profile { get; set; }
}