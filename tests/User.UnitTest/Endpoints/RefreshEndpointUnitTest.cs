using System.Net;
using System.Net.Http.Json;
using User.UnitTest.Factories;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Endpoints;

public class RefreshEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Refresh_WithValidTokens_ShouldReturn200AndNewTokens()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("refresh@example.com", "Password123!");
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Act
        var response = await DoPost("/api/auth/refresh", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.NotEqual(tokens.AccessToken, result.AccessToken);
        Assert.NotEqual(tokens.RefreshToken, result.RefreshToken);
    }

    [Fact]
    public async Task Refresh_WithInvalidRefreshToken_ShouldReturn400()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("invalid@example.com", "Password123!");
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: "invalid-refresh-token");

        // Act
        var response = await DoPost("/api/auth/refresh", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithRevokedToken_ShouldReturn400()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("revoked@example.com", "Password123!");
        
        // Revoke the token
        await TokenService.RevokeRefreshTokenAsync(tokens.RefreshToken, "192.168.1.1");

        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Act
        var response = await DoPost("/api/auth/refresh", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_AfterUsingRefreshToken_OldTokenShouldBeInvalid()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("once@example.com", "Password123!");
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Act - Use the refresh token once
        var response1 = await DoPost("/api/auth/refresh", request);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

        // Try to use the same refresh token again
        var response2 = await DoPost("/api/auth/refresh", request);

        // Assert - Second attempt should fail
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
    }
}

