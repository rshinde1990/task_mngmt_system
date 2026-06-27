using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TMS.Application.Interfaces;

namespace TMS.Application.Services;

public class AuthService : IAuthService
{
    private const string DemoUsername = "admin";
    private const string DemoPassword = "password123";

    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public Task<string> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        if (username != DemoUsername || password != DemoPassword)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var issuer   = _config["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var key      = _config["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
