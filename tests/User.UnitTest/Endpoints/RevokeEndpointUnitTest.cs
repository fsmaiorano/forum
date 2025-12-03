using System.Net;
using User.UnitTest.Factories;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Endpoints;

public class RevokeEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Revoke_WithValidRefreshToken_ShouldReturn204()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("revoke@example.com", "Password123!");
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Act
        var response = await DoPost("/api/auth/revoke", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify token is revoked in database
        var revokedToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);
        Assert.NotNull(revokedToken);
        Assert.NotNull(revokedToken.Revoked);
        Assert.False(revokedToken.IsActive);
    }

    [Fact]
    public async Task Revoke_WithInvalidRefreshToken_ShouldReturn204()
    {
        // Arrange
        var request = MakeUser.CreateRefreshRequest(
            accessToken: "some-token", 
            refreshToken: "invalid-refresh-token");

        // Act
        var response = await DoPost("/api/auth/revoke", request);

        // Assert - Should still return 204 (no content) even for invalid tokens
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Revoke_AlreadyRevokedToken_ShouldReturn204()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("already@example.com", "Password123!");
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Revoke once
        await DoPost("/api/auth/revoke", request);

        // Act - Revoke again
        var response = await DoPost("/api/auth/revoke", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Revoke_ThenRefresh_ShouldFail()
    {
        // Arrange
        var tokens = await CreateTestUserWithTokensAsync("revoke-then-refresh@example.com", "Password123!");
        var revokeRequest = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken, 
            refreshToken: tokens.RefreshToken);

        // Act - Revoke the token
        var revokeResponse = await DoPost("/api/auth/revoke", revokeRequest);
        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);

        // Try to refresh with revoked token
        var refreshResponse = await DoPost("/api/auth/refresh", revokeRequest);

        // Assert - Refresh should fail
        Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
    }
}

