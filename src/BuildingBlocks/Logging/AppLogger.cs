using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Logging;

public class AppLogger<T>(ILogger<T> logger) : IAppLogger<T>
{
    public void LogInformation(LogTypeEnum logTypeEnum, string message, params object[] args)
    {
        Log(LogLevel.Information, logTypeEnum, null, message, args);
    }

    public void LogWarning(LogTypeEnum logTypeEnum, string message, params object[] args)
    {
        Log(LogLevel.Warning, logTypeEnum, null, message, args);
    }

    public void LogError(LogTypeEnum logTypeEnum, Exception? exception, string message, params object[] args)
    {
        Log(LogLevel.Error, logTypeEnum, exception, message, args);
    }

    private void Log(LogLevel logLevel, LogTypeEnum logTypeEnum, Exception? exception, string message, params object[] args)
    {
        using (logger.BeginScope(new Dictionary<string, object> { ["LogType"] = logTypeEnum.ToString() }))
        {
            logger.Log(logLevel, exception, message, args);
        }
    }
}