using User.UnitTest.Factories;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Services;

public class TokenServiceUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task CreateTokensAsync_ShouldGenerateValidTokens()
    {
        // Arrange
        var user = await CreateTestUserAsync("test@example.com", "Password123!");
        var ipAddress = "192.168.1.1";

        // Act
        var result = await TokenService.CreateTokensAsync(user, ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.True(result.AccessTokenExpiration > DateTime.UtcNow);
        Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);

        // Verify refresh token is stored in database
        var storedToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == result.RefreshToken);
        Assert.NotNull(storedToken);
        Assert.Equal(user.Id, storedToken.UserId);
        Assert.Equal(ipAddress, storedToken.CreatedByIp);
    }

    [Fact]
    public async Task RefreshAsync_WithValidTokens_ShouldReturnNewTokenPair()
    {
        // Arrange
        var user = await CreateTestUserAsync("refresh@example.com", "Password123!");
        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");
        var ipAddress = "192.168.1.2";

        // Act
        var result = await TokenService.RefreshAsync(tokens.AccessToken, tokens.RefreshToken, ipAddress);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.NotEqual(tokens.AccessToken, result.AccessToken);
        Assert.NotEqual(tokens.RefreshToken, result.RefreshToken);

        // Verify old token is revoked
        var oldToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);
        Assert.NotNull(oldToken);
        Assert.NotNull(oldToken.Revoked);
        Assert.Equal(ipAddress, oldToken.RevokedByIp);

        // Verify new token is stored
        var newToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == result.RefreshToken);
        Assert.NotNull(newToken);
        Assert.True(newToken.IsActive);
    }

    [Fact]
    public async Task RefreshAsync_WithInvalidRefreshToken_ShouldReturnNull()
    {
        // Arrange
        var user = await CreateTestUserAsync("invalid@example.com", "Password123!");
        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");
        var invalidRefreshToken = "invalid-refresh-token";

        // Act
        var result = await TokenService.RefreshAsync(tokens.AccessToken, invalidRefreshToken, "192.168.1.2");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshAsync_WithRevokedRefreshToken_ShouldReturnNull()
    {
        // Arrange
        var user = await CreateTestUserAsync("revoked@example.com", "Password123!");
        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");
        
        // Revoke the token
        await TokenService.RevokeRefreshTokenAsync(tokens.RefreshToken, "192.168.1.1");

        // Act
        var result = await TokenService.RefreshAsync(tokens.AccessToken, tokens.RefreshToken, "192.168.1.2");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RefreshAsync_WithExpiredRefreshToken_ShouldReturnNull()
    {
        // Arrange
        var user = await CreateTestUserAsync("expired@example.com", "Password123!");
        var expiredToken = MakeUser.CreateRefreshToken(user.Id, expires: DateTime.UtcNow.AddDays(-1));
        Context.RefreshTokens.Add(expiredToken);
        await Context.SaveChangesAsync();

        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");

        // Act
        var result = await TokenService.RefreshAsync(tokens.AccessToken, expiredToken.Token, "192.168.1.2");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithValidToken_ShouldRevokeToken()
    {
        // Arrange
        var user = await CreateTestUserAsync("revoke@example.com", "Password123!");
        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");
        var ipAddress = "192.168.1.2";

        // Act
        await TokenService.RevokeRefreshTokenAsync(tokens.RefreshToken, ipAddress);

        // Assert
        var revokedToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);
        Assert.NotNull(revokedToken);
        Assert.NotNull(revokedToken.Revoked);
        Assert.Equal(ipAddress, revokedToken.RevokedByIp);
        Assert.False(revokedToken.IsActive);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithInvalidToken_ShouldNotThrowException()
    {
        // Arrange
        var invalidToken = "invalid-token";
        var ipAddress = "192.168.1.1";

        // Act & Assert - Should not throw
        await TokenService.RevokeRefreshTokenAsync(invalidToken, ipAddress);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithAlreadyRevokedToken_ShouldNotUpdateToken()
    {
        // Arrange
        var user = await CreateTestUserAsync("already-revoked@example.com", "Password123!");
        var tokens = await TokenService.CreateTokensAsync(user, "192.168.1.1");
        await TokenService.RevokeRefreshTokenAsync(tokens.RefreshToken, "192.168.1.2");

        var firstRevokedState = await Context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);

        // Act
        await TokenService.RevokeRefreshTokenAsync(tokens.RefreshToken, "192.168.1.3");

        // Assert
        var secondRevokedState = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);
        Assert.Equal(firstRevokedState!.Revoked, secondRevokedState!.Revoked);
        Assert.Equal(firstRevokedState.RevokedByIp, secondRevokedState.RevokedByIp);
    }
}

