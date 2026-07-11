using FCG.BuildingBlocks.Domain;
using FCG.BuildingBlocks.Enums;
using FCG.Users.Domain.Enums;
using FCG.Users.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace FCG.Users.Domain.Entities;

public class User : EntityBase
{
    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserProfile Profile { get; private set; }

    protected User()
    {
    }

    public static User Create(
     string name,
     string email,
     string passwordHash,
     UserProfile profile)
    {
        var normalizedName = name.Trim();
        var normalizedEmail = email.Trim().ToLower();

        ValidateName(normalizedName);
        ValidateEmail(normalizedEmail);
        ValidatePasswordHash(passwordHash);

        return new User
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            Profile = profile,
            Status = StatusType.Active,
            CreatedAt = DateTime.UtcNow
        };
    }


    public void Update(string name, string email)
    {
        EnsureActive();

        var normalizedName = name.Trim();
        var normalizedEmail = email.Trim().ToLower();

        ValidateName(normalizedName);
        ValidateEmail(normalizedEmail);

        Name = normalizedName;
        Email = normalizedEmail;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        EnsureActive();

        ValidatePasswordHash(passwordHash);

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeProfile(UserProfile profile)
    {
        EnsureActive();

        Profile = profile;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == StatusType.Active)
            throw new DomainException("Usuário já está ativo.");

        Status = StatusType.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Inactivate()
    {
        if (Status == StatusType.Inactive)
            throw new DomainException("Usuário já está inativo.");

        Status = StatusType.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email é obrigatório.");

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        if (!emailRegex.IsMatch(email))
            throw new DomainException("Formato de email inválido.");
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Senha é obrigatória.");
    }

    private void EnsureActive()
    {
        if (Status != StatusType.Active)
            throw new DomainException("Usuário está inativo.");
    }
}