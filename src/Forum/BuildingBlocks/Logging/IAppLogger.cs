namespace Forum.BuildingBlocks.Logging;

public interface IAppLogger<T>
{
    void LogInformation(LogType logType, string message, params object[] args);
    void LogWarning(LogType logType, string message, params object[] args);
    void LogError(LogType logType, Exception? exception, string message, params object[] args);
}

public class AppLogger<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public AppLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

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
        using (_logger.BeginScope(new Dictionary<string, object> { ["LogType"] = logType.ToString() }))
        {
            _logger.Log(logLevel, exception, message, args);
        }
    }
}

