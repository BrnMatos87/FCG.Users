namespace FCG.Users.Application.Contracts;

public interface IPasswordPolicy
{
    void Validate(string password);
}