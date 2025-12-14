using Bogus;
using User.Data.Models;

namespace User.UnitTest.Factories;

public static class MakeUser
{
    private static readonly Faker Faker = new();

    public static RegisterRequest CreateRegisterRequest(
        string? email = null,
        string? password = null)
    {
        return new RegisterRequest
        {
            Email = email ?? Faker.Internet.Email(),
            Password = password ?? Faker.Internet.Password(8, false, "", "@1Aa")
        };
    }

    public static AuthRequest CreateLoginRequest(
        string? email = null,
        string? password = null)
    {
        return new AuthRequest
        {
            Email = email ?? Faker.Internet.Email(),
            Password = password ?? Faker.Internet.Password(8)
        };
    }

    public static RefreshRequest CreateRefreshRequest(
        string? accessToken = null,
        string? refreshToken = null)
    {
        return new RefreshRequest
        {
            AccessToken = accessToken ?? Faker.Random.AlphaNumeric(100),
            RefreshToken = refreshToken ?? Faker.Random.AlphaNumeric(100)
        };
    }

    public static ApplicationUser CreateApplicationUser(
        string? email = null,
        string? userName = null)
    {
        var userEmail = email ?? Faker.Internet.Email();
        return new ApplicationUser
        {
            Email = userEmail,
            UserName = userName ?? userEmail,
            EmailConfirmed = true
        };
    }

    public static RefreshToken CreateRefreshToken(
        string userId,
        string? token = null,
        DateTime? expires = null,
        bool isRevoked = false)
    {
        var refreshToken = new RefreshToken
        {
            Token = token ?? Convert.ToBase64String(Faker.Random.Bytes(64)),
            UserId = userId,
            Expires = expires ?? DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            CreatedByIp = Faker.Internet.Ip()
        };

        if (!isRevoked) return refreshToken;
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = Faker.Internet.Ip();

        return refreshToken;
    }
}