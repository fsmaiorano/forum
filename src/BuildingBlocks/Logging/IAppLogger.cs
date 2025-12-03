namespace BuildingBlocks.Logging;

public interface IAppLogger<T>
{
    void LogInformation(LogTypeEnum logTypeEnum, string message, params object[] args);
    void LogWarning(LogTypeEnum logTypeEnum, string message, params object[] args);
    void LogError(LogTypeEnum logTypeEnum, Exception? exception, string message, params object[] args);
}