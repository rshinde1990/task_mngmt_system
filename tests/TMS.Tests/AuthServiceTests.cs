using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TMS.Application.Services;

namespace TMS.Tests;

public class AuthServiceTests
{
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        configMock.Setup(c => c["Jwt:Key"]).Returns("TestSigningKey_MustBeAtLeast32Chars!!");

        _sut = new AuthService(configMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsValid()
    {
        var token = await _sut.LoginAsync("admin", "password123");

        token.Should().NotBeNullOrWhiteSpace();
        token.Should().StartWith("eyJ", because: "a JWT always begins with the base64-encoded header");
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenCredentialsInvalid()
    {
        var act = async () => await _sut.LoginAsync("admin", "wrongpassword");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
