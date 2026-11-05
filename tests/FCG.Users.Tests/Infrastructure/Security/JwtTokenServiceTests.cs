using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using FCG.Users.Infrastructure.Security;
using Microsoft.Extensions.Options;

namespace FCG.Users.Tests.Infrastructure.Security;

public class JwtTokenServiceTests
{
    [Fact(DisplayName = "Validando geração de token JWT")]
    [Trait("Categoria", "Infrastructure - Security")]
    public void JwtToken_Generate_Success()
    {
        var options = Options.Create(new JwtOptions
        {
            SecretKey = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
            Issuer = "FCG.Users.Api",
            Audience = "FCG.CloudGames",
            ExpirationMinutes = 60
        });

        var service = new JwtTokenService(options);

        var user = User.Create(
            "Bruno",
            "bruno@email.com",
            "hash",
            UserProfile.Administrator);

        var token = service.Generate(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }
}