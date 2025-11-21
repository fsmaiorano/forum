namespace UnitTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public ForumDbContext Context { get; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ForumDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}


