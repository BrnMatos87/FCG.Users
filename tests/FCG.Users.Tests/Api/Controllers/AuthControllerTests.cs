using FCG.Users.Api.Controllers;
using FCG.Users.Api.DTOs.Requests;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Commands.Auth;
using FCG.Users.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FCG.Users.Tests.Api.Controllers;

public class AuthControllerTests
{
    [Fact(DisplayName = "Validando login com sucesso")]
    [Trait("Categoria", "API - AuthController")]
    public async Task AuthController_Login_Success()
    {
        var handler = new Mock<ICommandHandler<LoginCommand, LoginResponse>>();

        handler.Setup(x => x.HandleAsync(
                It.IsAny<LoginCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResponse
            {
                Token = "token-jwt"
            });

        var controller = new AuthController(handler.Object);

        var result = await controller.Login(new LoginRequest
        {
            Email = "admin@fcg.com",
            Password = "Admin@123"
        }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);

        Assert.Equal("token-jwt", response.Token);
    }
}