using Bogus;

namespace User.UnitTest.Factories;

public static class MakeUser
{
    private static readonly Faker _faker = new Faker();

    public static RegisterRequest CreateRegisterRequest(
        string? email = null,
        string? password = null)
    {
        return new RegisterRequest
        {
            Email = email ?? _faker.Internet.Email(),
            Password = password ?? _faker.Internet.Password(8, false, "", "@1Aa")
        };
    }

    public static AuthRequest CreateLoginRequest(
        string? email = null,
        string? password = null)
    {
        return new AuthRequest
        {
            Email = email ?? _faker.Internet.Email(),
            Password = password ?? _faker.Internet.Password(8)
        };
    }

    public static RefreshRequest CreateRefreshRequest(
        string? accessToken = null,
        string? refreshToken = null)
    {
        return new RefreshRequest
        {
            AccessToken = accessToken ?? _faker.Random.AlphaNumeric(100),
            RefreshToken = refreshToken ?? _faker.Random.AlphaNumeric(100)
        };
    }

    public static ApplicationUser CreateApplicationUser(
        string? email = null,
        string? userName = null)
    {
        var userEmail = email ?? _faker.Internet.Email();
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
            Token = token ?? Convert.ToBase64String(_faker.Random.Bytes(64)),
            UserId = userId,
            Expires = expires ?? DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            CreatedByIp = _faker.Internet.Ip()
        };

        if (isRevoked)
        {
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = _faker.Internet.Ip();
        }

        return refreshToken;
    }
}

