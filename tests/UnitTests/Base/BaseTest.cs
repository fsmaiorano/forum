using Forum.BuildingBlocks.Logging;

namespace UnitTests.Base;

public abstract class BaseTest(TestFixture fixture) : IClassFixture<TestFixture>
{
    protected ForumDbContext Context => fixture.GetDbContext();
    
    protected static Mock<IAppLogger<T>> CreateLoggerMock<T>() => new();
    
    protected readonly HttpClient HttpClient = fixture.CreateClient();
    
    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "", string culture = "en-US")
        => await fixture.DoPost(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
        => await fixture.DoGet(method, token, culture);
    
    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "", string culture = "en-US")
        => await fixture.DoPut(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "", string culture = "en-US")
        => await fixture.DoPatch(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
        => await fixture.DoDelete(method, token, culture);
}