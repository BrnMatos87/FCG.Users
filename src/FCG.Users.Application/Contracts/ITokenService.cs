using FCG.Users.Domain.Entities;

namespace FCG.Users.Application.Contracts;

public interface ITokenService
{
    string Generate(User user);
}