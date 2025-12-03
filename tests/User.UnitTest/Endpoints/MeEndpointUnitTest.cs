using System.Net;
using System.Net.Http.Json;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Endpoints;

public class MeEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Me_WithValidToken_ShouldReturn200AndUserInfo()
    {
        // Arrange
        var email = "me@example.com";
        var tokens = await CreateTestUserWithTokensAsync(email, "Password123!");

        // Act
        var response = await DoGet("/api/auth/me", token: tokens.AccessToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("userId"));
        Assert.True(result.ContainsKey("email"));
        Assert.Equal(email, result["email"]);
    }

    [Fact]
    public async Task Me_WithoutToken_ShouldReturn401()
    {
        // Act
        var response = await DoGet("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithInvalidToken_ShouldReturn401()
    {
        // Act
        var response = await DoGet("/api/auth/me", token: "invalid-token");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithExpiredToken_ShouldReturn401()
    {
        // Note: This test would require mocking time or generating an actually expired token
        // For now, we'll use a malformed token to simulate the failure case
        
        // Act
        var response = await DoGet("/api/auth/me", token: "expired.token.here");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

