using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.Application.Interfaces;
using TMS.Shared;

namespace TMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<string>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password, ct);
        return Ok(ApiResponse<string>.Ok(token, "Login successful."));
    }
}

public record LoginRequest(string Username, string Password);
