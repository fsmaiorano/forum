namespace Forum.BuildingBlocks.Logging;

public interface IAppLogger<T>
{
    void LogInformation(LogType logType, string message, params object[] args);
    void LogWarning(LogType logType, string message, params object[] args);
    void LogError(LogType logType, Exception? exception, string message, params object[] args);
}