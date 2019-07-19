
public enum DebugLogType
{
    Info,
    Warning,
    Error,
    Exception,
}

public interface ILogService 
{
    void Log(DebugLogType logType, string message);
}