namespace UnitTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public ForumDbContext Context { get; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ForumDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.CreateVersion7().ToString())
            .Options;

        Context = new ForumDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}


