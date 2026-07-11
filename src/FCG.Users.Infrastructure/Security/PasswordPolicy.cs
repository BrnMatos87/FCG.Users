using System.Text.RegularExpressions;
using FCG.Users.Application.Contracts;
using FCG.Users.Domain.Exceptions;

namespace FCG.Users.Infrastructure.Security;

public class PasswordPolicy : IPasswordPolicy
{
    public void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password is required.");

        if (password.Length < 8)
            throw new DomainException("Password must have at least 8 characters.");

        if (!Regex.IsMatch(password, @"[A-Za-z]"))
            throw new DomainException("Password must contain at least one letter.");

        if (!Regex.IsMatch(password, @"\d"))
            throw new DomainException("Password must contain at least one number.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            throw new DomainException("Password must contain at least one special character.");
    }
}