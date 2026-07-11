using System.ComponentModel;

namespace FCG.Users.Domain.Enums;

public enum UserProfile
{
    [Description("Usuario")]
    User = 1,
    [Description("Administrador")]
    Administrator = 2
}