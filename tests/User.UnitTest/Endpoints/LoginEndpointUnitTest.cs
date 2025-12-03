using System.Net;
using System.Net.Http.Json;
using User.UnitTest.Factories;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Endpoints;

public class LoginEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200AndTokens()
    {
        // Arrange
        var email = "login@example.com";
        var password = "Password123!";
        await CreateTestUserAsync(email, password);

        var request = MakeUser.CreateLoginRequest(email: email, password: password);

        // Act
        var response = await DoPost("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.True(result.AccessTokenExpiration > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ShouldReturn401()
    {
        // Arrange
        var request = MakeUser.CreateLoginRequest(email: "nonexistent@example.com", password: "Password123!");

        // Act
        var response = await DoPost("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturn401()
    {
        // Arrange
        var email = "wrongpass@example.com";
        var correctPassword = "CorrectPassword123!";
        await CreateTestUserAsync(email, correctPassword);

        var request = MakeUser.CreateLoginRequest(email: email, password: "WrongPassword123!");

        // Act
        var response = await DoPost("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_MultipleTimes_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var email = "multiple@example.com";
        var password = "Password123!";
        await CreateTestUserAsync(email, password);

        var request = MakeUser.CreateLoginRequest(email: email, password: password);

        // Act
        var response1 = await DoPost("/api/auth/login", request);
        var result1 = await response1.Content.ReadFromJsonAsync<AuthResponse>();

        var response2 = await DoPost("/api/auth/login", request);
        var result2 = await response2.Content.ReadFromJsonAsync<AuthResponse>();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.AccessToken, result2.AccessToken);
        Assert.NotEqual(result1.RefreshToken, result2.RefreshToken);
    }
}

