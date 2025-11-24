using Forum.BuildingBlocks.Logging;
using UnitTests.Factories;

namespace UnitTests.Base;

public abstract class BaseTest(DatabaseFixture databaseFixture, HttpFixture httpFixture)
    : IClassFixture<DatabaseFixture>, IClassFixture<HttpFixture>
{
    protected readonly ForumDbContext Context = databaseFixture.Context;
    protected static Mock<IAppLogger<T>> CreateLoggerMock<T>() => new();
    protected readonly HttpClient HttpClient = httpFixture.CreateClient();
    protected readonly HttpFixture HttpFixture = httpFixture;
    
    protected async Task<HttpResponseMessage> DoPost(string method, object request, string token = "", string culture = "en-US")
        => await HttpFixture.DoPost(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoGet(string method, string token = "", string culture = "en-US")
        => await HttpFixture.DoGet(method, token, culture);
    
    protected async Task<HttpResponseMessage> DoPut(string method, object request, string token = "", string culture = "en-US")
        => await HttpFixture.DoPut(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoPatch(string method, object request, string token = "", string culture = "en-US")
        => await HttpFixture.DoPatch(method, request, token, culture);
    
    protected async Task<HttpResponseMessage> DoDelete(string method, string token = "", string culture = "en-US")
        => await HttpFixture.DoDelete(method, token, culture);
}