using FCG.BuildingBlocks.Enums;
using FCG.Users.Domain.Entities;
using FCG.Users.Domain.Enums;
using FCG.Users.Domain.Exceptions;

namespace FCG.Users.Tests.Domain.Entities;

public class UserTests
{
    [Fact(DisplayName = "Validando se o nome está vazio")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Validate_Name_Empty()
    {
        var result = Assert.Throws<DomainException>(() =>
            User.Create(
                string.Empty,
                "bruno@email.com",
                "hash-da-senha",
                UserProfile.User));

        Assert.Equal("Nome é obrigatório.", result.Message);
    }

    [Fact(DisplayName = "Validando se o e-mail está vazio")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Validate_Email_Empty()
    {
        var result = Assert.Throws<DomainException>(() =>
            User.Create(
                "Bruno",
                string.Empty,
                "hash-da-senha",
                UserProfile.User));

        Assert.Equal("Email é obrigatório.", result.Message);
    }

    [Fact(DisplayName = "Validando se o e-mail é inválido")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Validate_Email_Invalid()
    {
        var result = Assert.Throws<DomainException>(() =>
            User.Create(
                "Bruno",
                "email-invalido",
                "hash-da-senha",
                UserProfile.User));

        Assert.Equal("Formato de email inválido.", result.Message);
    }

    [Fact(DisplayName = "Validando se a senha está vazia")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Validate_Password_Empty()
    {
        var result = Assert.Throws<DomainException>(() =>
            User.Create(
                "Bruno",
                "bruno@email.com",
                string.Empty,
                UserProfile.User));

        Assert.Equal("Senha é obrigatória.", result.Message);
    }

    [Fact(DisplayName = "Validando se o usuário foi criado com sucesso")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Create_Success()
    {
        var user = User.Create(
            " Bruno ",
            " BRUNO@EMAIL.COM ",
            "hash-da-senha",
            UserProfile.User);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Bruno", user.Name);
        Assert.Equal("bruno@email.com", user.Email);
        Assert.Equal("hash-da-senha", user.PasswordHash);
        Assert.Equal(UserProfile.User, user.Profile);
        Assert.Equal(StatusType.Active, user.Status);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando atualização de usuário com nome vazio")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Name_Empty()
    {
        var user = CreateValidUser();

        var result = Assert.Throws<DomainException>(() =>
            user.Update(string.Empty, "novo@email.com"));

        Assert.Equal("Nome é obrigatório.", result.Message);
    }

    [Fact(DisplayName = "Validando atualização de usuário com e-mail vazio")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Email_Empty()
    {
        var user = CreateValidUser();

        var result = Assert.Throws<DomainException>(() =>
            user.Update("Bruno Silva", string.Empty));

        Assert.Equal("Email é obrigatório.", result.Message);
    }

    [Fact(DisplayName = "Validando atualização de usuário com e-mail inválido")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Email_Invalid()
    {
        var user = CreateValidUser();

        var result = Assert.Throws<DomainException>(() =>
            user.Update("Bruno Silva", "email-invalido"));

        Assert.Equal("Formato de email inválido.", result.Message);
    }

    [Fact(DisplayName = "Validando atualização de usuário com sucesso")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Success()
    {
        var user = CreateValidUser();

        user.Update(" Bruno Silva ", " BRUNO.SILVA@EMAIL.COM ");

        Assert.Equal("Bruno Silva", user.Name);
        Assert.Equal("bruno.silva@email.com", user.Email);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando alteração de perfil com sucesso")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Profile_Success()
    {
        var user = CreateValidUser();

        user.ChangeProfile(UserProfile.Administrator);

        Assert.Equal(UserProfile.Administrator, user.Profile);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando alteração de senha vazia")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Password_Empty()
    {
        var user = CreateValidUser();

        var result = Assert.Throws<DomainException>(() =>
            user.ChangePassword(string.Empty));

        Assert.Equal("Senha é obrigatória.", result.Message);
    }

    [Fact(DisplayName = "Validando alteração de senha com sucesso")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Update_Password_Success()
    {
        var user = CreateValidUser();

        user.ChangePassword("novo-hash");

        Assert.Equal("novo-hash", user.PasswordHash);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando inativação de usuário")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Inactivate_Success()
    {
        var user = CreateValidUser();

        user.Inactivate();

        Assert.Equal(StatusType.Inactive, user.Status);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando usuário já inativo")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Inactivate_Already_Inactive()
    {
        var user = CreateValidUser();

        user.Inactivate();

        var result = Assert.Throws<DomainException>(() =>
            user.Inactivate());

        Assert.Equal("Usuário já está inativo.", result.Message);
    }

    [Fact(DisplayName = "Validando ativação de usuário")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Activate_Success()
    {
        var user = CreateValidUser();

        user.Inactivate();
        user.Activate();

        Assert.Equal(StatusType.Active, user.Status);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "Validando usuário já ativo")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Activate_Already_Active()
    {
        var user = CreateValidUser();

        var result = Assert.Throws<DomainException>(() =>
            user.Activate());

        Assert.Equal("Usuário já está ativo.", result.Message);
    }

    [Fact(DisplayName = "Validando que usuário inativo não pode ser atualizado")]
    [Trait("Categoria", "Validando Usuário")]
    public void User_Inactive_Cannot_Update()
    {
        var user = CreateValidUser();

        user.Inactivate();

        var result = Assert.Throws<DomainException>(() =>
            user.Update("Bruno Silva", "novo@email.com"));

        Assert.Equal("Usuário está inativo.", result.Message);
    }

    private static User CreateValidUser()
    {
        return User.Create(
            "Bruno",
            "bruno@email.com",
            "hash-da-senha",
            UserProfile.User);
    }
}