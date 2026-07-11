using FCG.Users.Infrastructure.Security;

namespace FCG.Users.Tests.Infrastructure.Security;

public class BCryptPasswordHasherTests
{
    [Fact(DisplayName = "Validando geração de hash da senha")]
    [Trait("Categoria", "Infrastructure - Security")]
    public void PasswordHasher_Hash_Success()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("Senha@123");

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual("Senha@123", hash);
    }

    [Fact(DisplayName = "Validando senha correta")]
    [Trait("Categoria", "Infrastructure - Security")]
    public void PasswordHasher_Verify_Success()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("Senha@123");

        var result = hasher.Verify("Senha@123", hash);

        Assert.True(result);
    }

    [Fact(DisplayName = "Validando senha incorreta")]
    [Trait("Categoria", "Infrastructure - Security")]
    public void PasswordHasher_Verify_InvalidPassword()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("Senha@123");

        var result = hasher.Verify("SenhaErrada", hash);

        Assert.False(result);
    }
}