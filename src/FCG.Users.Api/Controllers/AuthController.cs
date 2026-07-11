using FCG.Users.Api.DTOs.Requests;
using FCG.Users.Api.Mappers;
using FCG.Users.Application.Abstractions.Commands;
using FCG.Users.Application.Commands.Auth;
using FCG.Users.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<LoginCommand, LoginResponse> _loginCommandHandler;

    public AuthController(ICommandHandler<LoginCommand, LoginResponse> loginCommandHandler)
    {
        _loginCommandHandler = loginCommandHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var response = await _loginCommandHandler.HandleAsync(
            request.ToCommand(),
            ct);

        return Ok(response);
    }
}