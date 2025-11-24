using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Fixtures;

public class WebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<ForumDbContext>(options => options.UseInMemoryDatabase("ForumInMemoryDb"));
        });

        base.ConfigureWebHost(builder);
    }

    public new Task DisposeAsync()
    {
        return base.DisposeAsync().AsTask();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}