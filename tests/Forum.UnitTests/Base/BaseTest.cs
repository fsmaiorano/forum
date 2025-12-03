using BuildingBlocks.Logging;
using BuildingBlocks.Messaging.DomainEvents;
using Forum.UnitTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.UnitTests.Base;

public abstract class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>, IDisposable
{
    private readonly IServiceScope _scope = fixture.Services.CreateScope();
    
    protected IDomainEventDispatcher DomainEventDispatcher => _scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
    protected ForumDbContext Context => _scope.ServiceProvider.GetRequiredService<ForumDbContext>();
    protected TestFixture Fixture => fixture;

    protected static Mock<IAppLogger<T>> CreateLoggerMock<T>() => new();

    protected readonly HttpClient HttpClient = fixture.CreateClient();
    
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
    
    public void Dispose()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
    }

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
}