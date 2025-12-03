using System.Net;
using User.UnitTest.Factories;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Endpoints;

public class RevokeEndpointUnitTest(TestFixture fixture) : BaseTest(fixture)
{
    [Fact]
    public async Task Revoke_WithValidRefreshToken_ShouldReturn204()
    {
        var tokens = await CreateTestUserWithTokensAsync(faker.Internet.Email(),
            faker.Internet.Password(25, false, string.Empty, "1"));
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken,
            refreshToken: tokens.RefreshToken);

        var response = await DoPost("/api/auth/revoke", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var revokedToken = await Context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokens.RefreshToken);
        Assert.NotNull(revokedToken);
        Assert.NotNull(revokedToken.Revoked);
        Assert.False(revokedToken.IsActive);
    }

    [Fact]
    public async Task Revoke_WithInvalidRefreshToken_ShouldReturn204()
    {
        var request = MakeUser.CreateRefreshRequest(
            accessToken: "some-token",
            refreshToken: "invalid-refresh-token");

        var response = await DoPost("/api/auth/revoke", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Revoke_AlreadyRevokedToken_ShouldReturn204()
    {
        var tokens = await CreateTestUserWithTokensAsync(faker.Internet.Email(), faker.Internet.Password(25, false, string.Empty, "1"));
        var request = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken,
            refreshToken: tokens.RefreshToken);

        await DoPost("/api/auth/revoke", request);

        var response = await DoPost("/api/auth/revoke", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Revoke_ThenRefresh_ShouldFail()
    {
        var tokens = await CreateTestUserWithTokensAsync(faker.Internet.Email(), faker.Internet.Password(25, false, string.Empty, "1"));
        var revokeRequest = MakeUser.CreateRefreshRequest(
            accessToken: tokens.AccessToken,
            refreshToken: tokens.RefreshToken);

        var revokeResponse = await DoPost("/api/auth/revoke", revokeRequest);
        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);

        var refreshResponse = await DoPost("/api/auth/refresh", revokeRequest);

        Assert.Equal(HttpStatusCode.BadRequest, refreshResponse.StatusCode);
    }
}