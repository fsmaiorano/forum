using UnitTests.Factories;

namespace UnitTests.Base;

public abstract class BaseTest(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    protected readonly ForumDbContext Context = fixture.Context;
    protected static Mock<ILogger<T>> CreateLoggerMock<T>() => new();
}