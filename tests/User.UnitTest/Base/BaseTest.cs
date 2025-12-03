using Bogus;
using Microsoft.Extensions.DependencyInjection;
using User.UnitTest.Fixtures;

namespace User.UnitTest.Base;

public abstract class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IDisposable
{
    #region Fields and Properties

    private readonly IServiceScope _scope = fixture.Services.CreateScope();

    protected TestFixture Fixture => fixture;
    protected UserDbContext Context => _scope.ServiceProvider.GetRequiredService<UserDbContext>();

    protected UserManager<ApplicationUser> UserManager =>
        _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    protected SignInManager<ApplicationUser> SignInManager =>
        _scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

    protected ITokenService TokenService => _scope.ServiceProvider.GetRequiredService<ITokenService>();
    protected HttpClient HttpClient { get; } = fixture.CreateClient();
    protected Faker faker = new Faker();

    #endregion

    #region HTTP Request Helpers

    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPost(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
        => await fixture.DoGet(method, token, culture);

    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPut(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "",
        string culture = "en-US")
        => await fixture.DoPatch(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
        => await fixture.DoDelete(method, token, culture);

    #endregion

    #region Helper Methods

    protected async Task<ApplicationUser> CreateTestUserAsync(string email, string password)
    {
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await UserManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception(
                $"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        return user;
    }

    protected async Task<AuthResponse> CreateTestUserWithTokensAsync(string email, string password)
    {
        var user = await CreateTestUserAsync(email, password);
        return await TokenService.CreateTokensAsync(user, faker.Internet.Ip());
    }

    #endregion

    #region Lifecycle

    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}