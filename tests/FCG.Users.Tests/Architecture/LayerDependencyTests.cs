using FCG.Users.Application.Commands.Users;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using NetArchTest.Rules;

namespace FCG.Users.Tests.Architecture;

public class LayerDependencyTests
{
    private const string DomainNamespace = "FCG.Users.Domain";
    private const string ApplicationNamespace = "FCG.Users.Application";
    private const string InfrastructureNamespace = "FCG.Users.Infrastructure";
    private const string ApiNamespace = "FCG.Users.Api";

    [Fact(DisplayName = "Domain não deve depender de Application")]
    [Trait("Categoria", "Architecture")]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var result = Types
            .InAssembly(typeof(User).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Domain não deve depender de Infrastructure")]
    [Trait("Categoria", "Architecture")]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types
            .InAssembly(typeof(User).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Domain não deve depender de Api")]
    [Trait("Categoria", "Architecture")]
    public void Domain_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(typeof(User).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Application não deve depender de Infrastructure")]
    [Trait("Categoria", "Architecture")]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types
            .InAssembly(typeof(CreateUserCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Application não deve depender de Api")]
    [Trait("Categoria", "Architecture")]
    public void Application_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(typeof(CreateUserCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Infrastructure não deve depender de Api")]
    [Trait("Categoria", "Architecture")]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(typeof(UsersDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Domain não deve depender do Entity Framework")]
    [Trait("Categoria", "Architecture")]
    public void Domain_Should_Not_Depend_On_EntityFramework()
    {
        var result = Types
            .InAssembly(typeof(User).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact(DisplayName = "Application não deve depender do Entity Framework")]
    [Trait("Categoria", "Architecture")]
    public void Application_Should_Not_Depend_On_EntityFramework()
    {
        var result = Types
            .InAssembly(typeof(CreateUserCommand).Assembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}