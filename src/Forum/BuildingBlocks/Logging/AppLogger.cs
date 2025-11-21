namespace Forum.BuildingBlocks.Logging;

public class AppLogger<T>(ILogger<T> logger) : IAppLogger<T>
{
    public void LogInformation(LogType logType, string message, params object[] args)
    {
        Log(LogLevel.Information, logType, null, message, args);
    }

    public void LogWarning(LogType logType, string message, params object[] args)
    {
        Log(LogLevel.Warning, logType, null, message, args);
    }

    public void LogError(LogType logType, Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Error, logType, exception, message, args);
    }

    private void Log(LogLevel logLevel, LogType logType, Exception? exception, string message, params object[] args)
    {
        using (logger.BeginScope(new Dictionary<string, object> { ["LogType"] = logType.ToString() }))
        {
            logger.Log(logLevel, exception, message, args);
        }
    }
}