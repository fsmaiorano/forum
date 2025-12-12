using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.DomainEvents;
using Forum.UnitTest.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.UnitTest.Base;

public abstract class BaseTest : IClassFixture<TestFixture>, IDisposable
{
    #region Fields and Properties

    private readonly IServiceScope _scope;

    private TestFixture Fixture { get; }

    protected ForumDbContext Context => _scope.ServiceProvider.GetRequiredService<ForumDbContext>();

    protected IDomainEventDispatcher DomainEventDispatcher =>
        _scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();

    protected HttpClient HttpClient { get; }

    #endregion

    protected BaseTest(TestFixture fixture)
    {
        Fixture = fixture;
        _scope = fixture.Services.CreateScope();
        HttpClient = fixture.CreateClient();

        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    #region Domain Event Helpers

    protected TestDomainEventDispatcher GetTestDispatcher()
    {
        var dispatcher = DomainEventDispatcher as TestDomainEventDispatcher;
        if (dispatcher == null)
            throw new InvalidOperationException("DomainEventDispatcher is not a TestDomainEventDispatcher");
        return dispatcher;
    }

    protected void ClearDomainEvents()
    {
        var dispatcher = DomainEventDispatcher as TestDomainEventDispatcher;
        dispatcher?.Clear();
    }

    #endregion

    #region Mock Helpers

    protected static Mock<IAppLogger<T>> CreateLoggerMock<T>() => new();

    #endregion

    #region HTTP Request Helpers

    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "",
        string culture = "en-US")
        => await Fixture.DoPost(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
        => await Fixture.DoGet(method, token, culture);

    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "",
        string culture = "en-US")
        => await Fixture.DoPut(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "",
        string culture = "en-US")
        => await Fixture.DoPatch(method, request, token, culture);

    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
        => await Fixture.DoDelete(method, token, culture);

    #endregion

    #region Lifecycle

    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}