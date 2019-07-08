
public enum DebugLogType
{
    Info,
    Warning,
    Error,
    Exception,
}

public interface ILogService : IService
{
    void Log(DebugLogType logType, string message);
}